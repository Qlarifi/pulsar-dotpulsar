namespace DotPulsar.Internal.Encryption;

public class EncryptionKeyInfo(byte[] key, Dictionary<string, string> metadata)
{
    /// <summary>
    /// This object contains the encryption key and corresponding metadata which contains
    /// additional information about the key such as version, timestamp.
    /// </summary>
    public Dictionary<string, string> Metadata { get; } = metadata;

    public byte[] Key { get; } = key;
}
