namespace DotPulsar.Tests.Internal.Encryption;

using DotPulsar.Internal.Encryption;
using System.Buffers;

public class MessageCryptoTests
{
    public class Encrypt
    {
        private readonly MessageCrypto _sut;

        public Encrypt()
        {
            _sut = new MessageCrypto();
        }

        [Fact]
        public void ShouldEncrypt()
        {
            // Arrange
            var data = new ReadOnlySequence<byte>(new byte[] { 1, 2, 3 });

            // Act
            var (encryptedPayload, nonce, encryptedDataKeys) = _sut.Encrypt(data);

            // Assert
            encryptedPayload.Length.Should().Be(3);
            nonce.Length.Should().Be(0);
            encryptedDataKeys.Count.Should().Be(0);
        }
    }

    public class Decrypt
    {
        private readonly MessageCrypto _sut;

        public Decrypt()
        {
            _sut = new MessageCrypto();
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
