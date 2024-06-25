namespace DotPulsar.Tests.Internal.Encryption;

using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using DotPulsar.Internal.Abstractions;

public class AwsNonceProvider(AmazonKeyManagementServiceClient kmsClient) : INonceProvider
{
    private byte[]? _iv;

    public async Task<byte[]> GetNonce(int sizeInBytes)
    {
        if (_iv != null)
        {
            return _iv;
        }

        var randomRequest = new GenerateRandomRequest { NumberOfBytes = sizeInBytes };
        var randomResult = await kmsClient.GenerateRandomAsync(randomRequest);
        _iv = randomResult.Plaintext.ToArray();

        return _iv;
    }
}
