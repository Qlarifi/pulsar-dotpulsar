namespace DotPulsar.Tests.Internal.Encryption;

using DotPulsar.Internal.Encryption;
using System.Collections.Concurrent;

public class EncryptionKeysConverterTests
{
    public class FromDictionary
    {
        [Fact]
        public void Given_When_Should()
        {
            // Arrange
            var dic = new ConcurrentDictionary<string, EncryptionKeyInfo>();

            // Act
            var result = EncryptionKeysConverter.FromDictionary(dic);

            // Assert
            result.Count.Should().Be(-1);
        }
    }
}
