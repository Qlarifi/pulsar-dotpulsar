namespace DotPulsar.Internal.Encryption;
#if !NETSTANDARD2_0
using DotPulsar.Internal.Abstractions;
using System.Security.Cryptography;

public class LocalNonceProvider : INonceProvider
{
    private byte[]? _iv;

    public Task<byte[]> GetNonce(int sizeInBytes)
    {
        if (_iv != null)
        {
            return Task.FromResult(_iv);
        }

        _iv = new byte[sizeInBytes];
        RandomNumberGenerator.Fill(_iv);

        return Task.FromResult(_iv);
    }
}
#endif
