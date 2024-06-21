namespace DotPulsar.Internal.Encryption;

using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System.Buffers;

public class MessageCrypto : IMessageCrypto
{
    public (ReadOnlySequence<byte>, byte[], List<EncryptionKeys>) Encrypt(ReadOnlySequence<byte> plainTextBytes)
    {
        return (plainTextBytes, [], []);
    }

    public ReadOnlySequence<byte> Decrypt(ReadOnlySequence<byte> cipherTextBytes)
    {
        return cipherTextBytes;
    }
}
