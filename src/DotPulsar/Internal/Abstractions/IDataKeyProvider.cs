namespace DotPulsar.Internal.Abstractions;

public interface IDataKeyProvider
{
    Task<byte[]> GetDataKey(int sizeInBytes);
}
