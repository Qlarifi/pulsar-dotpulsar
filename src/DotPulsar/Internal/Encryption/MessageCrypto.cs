namespace DotPulsar.Internal.Encryption;
#if !NETSTANDARD2_0
using DotPulsar.Internal.Abstractions;
using DotPulsar.Internal.PulsarApi;
using System.Buffers;
using System.Security.Cryptography;

public class MessageCrypto(
    DataKeyManager dataKeyManager,
    List<string> encryptionKeyNames)
    : IMessageCrypto
{
    private static readonly int TagSize = AesGcm.TagByteSizes.MaxSize;

    public async Task<(ReadOnlySequence<byte>, byte[], List<EncryptionKeys>)> Encrypt(ReadOnlySequence<byte> plainTextBytes)
    {
        try
        {
            var dataKey = await dataKeyManager.GetDataKey();
            var encryptedDataKeys = await dataKeyManager.RefreshEncryptedDataKeys(dataKey, encryptionKeyNames);
            var nonce = await dataKeyManager.GetNonce();

            var cipherText = new byte[plainTextBytes.Length];
            var tag = new byte[TagSize];

            // Encrypt
#if NET8_0
            using var aesGcm = new AesGcm(dataKey, TagSize);
#endif
#if NET6_0 || NET7_0 || NETSTANDARD2_1
            using var aesGcm = new AesGcm(dataKey);
#endif
            aesGcm.Encrypt(nonce, plainTextBytes.ToArray(), cipherText, tag);

            return (new ReadOnlySequence<byte>(tag.Concat(cipherText).ToArray()), nonce, encryptedDataKeys);
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    public ReadOnlySequence<byte> Decrypt(ReadOnlySequence<byte> cipherTextBytes)
    {
        return cipherTextBytes;
    }
}

#endif
