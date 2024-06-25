namespace DotPulsar.Tests.Internal.Encryption;

using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using DotPulsar.Abstractions;
using DotPulsar.Internal.Encryption;

public class AwsDataKeyEncryptor(AmazonKeyManagementServiceClient kmsClient) : IDataKeyEncryptor
{
    public async Task<EncryptionKeyInfo> EncryptDataKey(byte[] dataKey, string keyId)
    {
        var encryptRequest = new EncryptRequest
        {
            KeyId = $"alias/{keyId}",
            Plaintext = new MemoryStream(dataKey),
            EncryptionAlgorithm = EncryptionAlgorithmSpec.RSAES_OAEP_SHA_256
        };
        var dataKeyResponse = await kmsClient.EncryptAsync(encryptRequest);
        var encryptedDataKey = dataKeyResponse.CiphertextBlob.ToArray();
        var encryptionKeyInfo = new EncryptionKeyInfo(encryptedDataKey, new Dictionary<string, string>());
        return encryptionKeyInfo;
    }

    public async Task<byte[]> DecryptDataKey(byte[] encryptedDataKey, string keyId)
    {
        var decryptRequest = new DecryptRequest
        {
            KeyId = $"alias/{keyId}",
            CiphertextBlob = new MemoryStream(encryptedDataKey),
            EncryptionAlgorithm = EncryptionAlgorithmSpec.RSAES_OAEP_SHA_256
        };
        var decryptResponse = await kmsClient.DecryptAsync(decryptRequest);
        var plainDataKey = decryptResponse.Plaintext.ToArray();
        return plainDataKey;
    }
}
