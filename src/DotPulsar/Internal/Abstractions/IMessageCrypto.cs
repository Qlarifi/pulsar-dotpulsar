namespace DotPulsar.Internal.Abstractions;

using DotPulsar.Internal.PulsarApi;
using System.Buffers;

public interface IMessageCrypto
{
    (ReadOnlySequence<byte>, byte[], List<EncryptionKeys>) Encrypt(ReadOnlySequence<byte> data);

    ReadOnlySequence<byte> Decrypt(ReadOnlySequence<byte> cipherTextBytes);
}
