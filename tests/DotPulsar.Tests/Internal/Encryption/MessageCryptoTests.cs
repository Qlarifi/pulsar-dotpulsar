namespace DotPulsar.Tests.Internal.Encryption;

using DotPulsar.Internal.Encryption;
using System.Buffers;
using System.Text;

public class MessageCryptoTests
{
    public class Encrypt
    {
        private readonly MessageCrypto _sut;

        public Encrypt()
        {
            var encryptionKeys = new List<string>();
            var dataKeyManager = new DataKeyManager(new LocalDataKeyEncryptor());

            _sut = new MessageCrypto(dataKeyManager, encryptionKeys);
        }

        [Fact]
        public async Task ShouldEncrypt()
        {
            // Arrange
            var data = new ReadOnlySequence<byte>(Encoding.UTF8.GetBytes("Hello World!"));

            // Act
            var (encryptedPayload, nonce, encryptedDataKeys) = await _sut.Encrypt(data);

            // Assert
            encryptedPayload.Length.Should().Be(28L);
            nonce.Length.Should().Be(12);
            encryptedDataKeys.Count.Should().Be(0);
        }
    }

    public class Decrypt
    {
        private readonly MessageCrypto _sut;

        public Decrypt()
        {
            var encryptionKeys = new List<string>();
            var dataKeyManager = new DataKeyManager(new LocalDataKeyEncryptor());

            _sut = new MessageCrypto(dataKeyManager, encryptionKeys);
        }

        [Fact]
        public void ShouldDecrypt()
        {
            // Arrange
            var data = new ReadOnlySequence<byte>([1, 2, 3]);

            // Act
            var result = _sut.Decrypt(data);

            // Assert
            result.Length.Should().Be(3);
        }
    }
}
