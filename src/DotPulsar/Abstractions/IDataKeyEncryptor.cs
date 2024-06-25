namespace DotPulsar.Abstractions;

using DotPulsar.Internal.Encryption;

public interface IDataKeyEncryptor
{
    public Task<EncryptionKeyInfo> EncryptDataKey(byte[] dataKey, string keyId);

    public Task<byte[]> DecryptDataKey(byte[] encryptedDataKey, string keyId);
}
