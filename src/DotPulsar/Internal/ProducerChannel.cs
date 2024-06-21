/*
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace DotPulsar.Internal;

using DotPulsar.Exceptions;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.Encryption;
using DotPulsar.Internal.PulsarApi;
using Microsoft.Extensions.ObjectPool;
using System.Buffers;

public sealed class ProducerChannel : IProducerChannel
{
    private readonly ObjectPool<SendPackage> _sendPackagePool;
    private readonly ulong _id;
    private readonly string _name;
    private readonly IConnection _connection;
    private readonly ICompressorFactory? _compressorFactory;
    private readonly IMessageCrypto? _messageCrypto;
    private readonly ProducerCryptoFailureAction _cryptoFailureAction;
    private readonly byte[]? _schemaVersion;

    public ProducerChannel(ulong id,
        string name,
        IConnection connection,
        ICompressorFactory? compressorFactory,
        IMessageCrypto? messageCrypto,
        ProducerCryptoFailureAction cryptoFailureAction,
        byte[]? schemaVersion)
    {
        var sendPackagePolicy = new DefaultPooledObjectPolicy<SendPackage>();
        _sendPackagePool = new DefaultObjectPool<SendPackage>(sendPackagePolicy);
        _id = id;
        _name = name;
        _connection = connection;
        _compressorFactory = compressorFactory;
        _messageCrypto = messageCrypto;
        _cryptoFailureAction = cryptoFailureAction;
        _schemaVersion = schemaVersion;
    }

    public async ValueTask ClosedByClient(CancellationToken cancellationToken)
    {
        try
        {
            var closeProducer = new CommandCloseProducer { ProducerId = _id };
            await _connection.Send(closeProducer, cancellationToken).ConfigureAwait(false);
        }
        catch
        {
            // Ignore
        }
    }

    public ValueTask DisposeAsync() => new();

    public async Task Send(MessageMetadata metadata, ReadOnlySequence<byte> payload, TaskCompletionSource<BaseCommand> responseTcs, CancellationToken cancellationToken)
    {
        var sendPackage = _sendPackagePool.Get();
        var resetSchema = false;
        var resetCompression = false;

        try
        {
            metadata.PublishTime = (ulong) DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            metadata.ProducerName = _name;

            if (metadata.SchemaVersion is null && _schemaVersion is not null)
            {
                metadata.SchemaVersion = _schemaVersion;
                resetSchema = true;
            }

            sendPackage.Command ??= new CommandSend { ProducerId = _id, NumMessages = 1 };

            sendPackage.Command.SequenceId = metadata.SequenceId;
            sendPackage.Metadata = metadata;

            if (_compressorFactory is null || metadata.Compression != CompressionType.None)
                sendPackage.Payload = payload;
            else
            {
                sendPackage.Metadata.Compression = _compressorFactory.CompressionType;
                sendPackage.Metadata.UncompressedSize = (uint) payload.Length;
                using var compressor = _compressorFactory.Create();
                sendPackage.Payload = compressor.Compress(payload);
                resetCompression = true;
            }

            if (_messageCrypto is not null)
            {
                try
                {
                    var (encryptedPayload, nonce, encryptedDataKeys) = _messageCrypto.Encrypt(sendPackage.Payload);

                    sendPackage.Payload = encryptedPayload;
                    sendPackage.Metadata.EncryptionParam = nonce;
                    sendPackage.Metadata.EncryptionKeys.AddRange(encryptedDataKeys);
                }
                catch (CryptoException cryptoException)
                {
                    if (_cryptoFailureAction == ProducerCryptoFailureAction.Send)
                    {
                        sendPackage.Payload = payload;
                        sendPackage.Metadata.EncryptionParam = null;
                        sendPackage.Metadata.EncryptionKeys.Clear();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            await _connection.Send(sendPackage, responseTcs, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            if (resetSchema)
                metadata.SchemaVersion = null;

            if (resetCompression)
            {
                metadata.Compression = CompressionType.None;
                metadata.UncompressedSize = 0;
            }

            _sendPackagePool.Return(sendPackage);
        }
    }
}
