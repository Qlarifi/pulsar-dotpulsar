namespace DotPulsar.Internal.Encryption;
#if !NETSTANDARD2_0
using DotPulsar.Internal.Abstractions;
using System.Security.Cryptography;

public class LocalDataKeyProvider : IDataKeyProvider
{
    public Task<byte[]> GetDataKey(int sizeInBytes)
    {
        var dataKey = new byte[sizeInBytes];
        RandomNumberGenerator.Fill(dataKey);

        return Task.FromResult(dataKey);
    }
}
#endif
