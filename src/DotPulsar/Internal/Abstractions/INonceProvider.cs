namespace DotPulsar.Internal.Abstractions;

public interface INonceProvider
{
    Task<byte[]> GetNonce(int sizeInBytes);
}
