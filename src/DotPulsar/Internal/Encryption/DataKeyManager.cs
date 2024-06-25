#if !NETSTANDARD2_0
namespace DotPulsar.Internal.Encryption;

using DotPulsar.Abstractions;
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System.Collections.Concurrent;
using System.Security.Cryptography;

public class DataKeyManager(
    IDataKeyEncryptor dataKeyEncryptor,
    INonceProvider nonceProvider,
    IDataKeyProvider dataKeyProvider)
{
    public DataKeyManager(IDataKeyEncryptor dataKeyEncryptor)
        : this(dataKeyEncryptor, new LocalNonceProvider(), new LocalDataKeyProvider()) { }

    /// <summary>
    /// This key is used to encrypt the message and needs to be refreshed every 4 hours or X number of messages.
    /// </summary>
    private byte[]? _dataKey;

    private readonly ConcurrentDictionary<String, EncryptionKeyInfo> _encryptedDataKeyMap = new();
    private const int MaxKeyLength = 32;
    private static readonly int NonceSize = AesGcm.NonceByteSizes.MaxSize;

    /// <summary>
    /// The data key is only encrypted once in a while.
    /// </summary>
    public async Task<byte[]> GetDataKey()
    {
        if (_dataKey is not null)
        {
            return _dataKey;
        }

        _dataKey = await dataKeyProvider.GetDataKey(MaxKeyLength);
        return _dataKey;
    }

    public async Task<List<EncryptionKeys>> RefreshEncryptedDataKeys(byte[] dataKey, List<string> encryptionKeyNames)
    {
        foreach (var keyName in encryptionKeyNames)
        {
            if (!_encryptedDataKeyMap.ContainsKey(keyName))
            {
                var encryptionKeyInfo = await dataKeyEncryptor.EncryptDataKey(dataKey, keyName);
                _encryptedDataKeyMap.TryAdd(keyName, encryptionKeyInfo);
            }
        }

        return EncryptionKeysConverter.FromDictionary(_encryptedDataKeyMap);
    }

    /// <summary>
    /// The nonce needs to be regenerated for each message encrypted.
    /// </summary>
    public async Task<byte[]> GetNonce()
    {
        var nonce = await nonceProvider.GetNonce(NonceSize);
        return nonce;
    }
}

#endif
