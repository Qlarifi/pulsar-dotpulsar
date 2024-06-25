namespace DotPulsar.Tests.Internal.Encryption;

using DotPulsar.Abstractions;
using DotPulsar.Internal.Encryption;
using System.Security.Cryptography;

public class LocalDataKeyEncryptor : IDataKeyEncryptor
{
#if NETSTANDARD2_0 || NETSTANDARD2_1
    public Task<EncryptionKeyInfo> EncryptDataKey(byte[] dataKey, string keyId)
    {
        return Task.FromResult(new EncryptionKeyInfo(dataKey, new Dictionary<string, string>()));
#else
    public async Task<EncryptionKeyInfo> EncryptDataKey(byte[] dataKey, string keyId)
    {
        var keyFileContent = await File.ReadAllTextAsync(keyId);
        var encryptor = RSA.Create();
        encryptor.ImportFromPem(keyFileContent);
        var encryptedDataKey = encryptor.Encrypt(dataKey, RSAEncryptionPadding.OaepSHA256);
        var encryptionKeyInfo = new EncryptionKeyInfo(encryptedDataKey, new Dictionary<string, string>());
        return encryptionKeyInfo;
#endif
    }

    public Task<byte[]> DecryptDataKey(byte[] encryptedDataKey, string keyId)
    {
        throw new NotImplementedException();
    }
}
