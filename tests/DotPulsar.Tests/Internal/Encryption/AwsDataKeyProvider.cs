namespace DotPulsar.Tests.Internal.Encryption;

using Amazon.KeyManagementService;
using Amazon.KeyManagementService.Model;
using DotPulsar.Internal.Abstractions;

public class AwsDataKeyProvider(AmazonKeyManagementServiceClient kmsClient) : IDataKeyProvider
{
    public async Task<byte[]> GetDataKey(int sizeInBytes)
    {
        var randomRequest = new GenerateRandomRequest { NumberOfBytes = sizeInBytes };
        var randomResult = await kmsClient.GenerateRandomAsync(randomRequest);
        var dataKey = randomResult.Plaintext.ToArray();
        return dataKey;
    }
}
