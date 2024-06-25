namespace DotPulsar.Internal.Encryption;

using DotPulsar.Internal.PulsarApi;
using System.Collections.Concurrent;

public static class EncryptionKeysConverter
{
    public static List<EncryptionKeys> FromDictionary(ConcurrentDictionary<string, EncryptionKeyInfo> encryptedDataKeys)
    {
        var encryptionKeys = new List<EncryptionKeys>();

        foreach (var encryptedDataKey in encryptedDataKeys)
        {
            var keyName = encryptedDataKey.Key;
            var encryptionKeyInfo = encryptedDataKey.Value;

            var encryptionKey = new EncryptionKeys
            {
                Key = keyName,
                Value = encryptionKeyInfo.Key
            };

            if (encryptionKeyInfo.Metadata.Any())
            {
                var keyValues = encryptionKeyInfo.Metadata
                    .Select(x => new KeyValue
                    {
                        Key = x.Key,
                        Value = x.Value
                    });

                encryptionKey.Metadatas.AddRange(keyValues);
            }

            encryptionKeys.Add(encryptionKey);
        }

        return encryptionKeys;
    }
}
