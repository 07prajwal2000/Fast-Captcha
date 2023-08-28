using System.Security.Cryptography;
using System.Text;

namespace FastCaptcha.Hashing;

public class ShaHashService
{
    private static readonly byte[] key = "this is key"u8.ToArray();

    public string HashData(string rawText, DateTimeOffset? expiry = null)
    {
        Span<byte> unixExpirySpan = stackalloc byte[8];

        var unixExpiry = expiry?.ToUnixTimeSeconds() ?? DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds();
        BitConverter.TryWriteBytes(unixExpirySpan[..8], unixExpiry); // writing unix_timestamp to hash
        
        Span<byte> rawTextAsSpan = stackalloc byte[rawText.Length];
        Encoding.UTF8.GetBytes(rawText, rawTextAsSpan); // writing raw_text to the hash as bytes

        Span<byte> combined = stackalloc byte[unixExpirySpan.Length + rawTextAsSpan.Length];
        unixExpirySpan.CopyTo(combined[..unixExpirySpan.Length]); // copying the unix_timestamp and captcha_text for hashing
        rawTextAsSpan.CopyTo(combined[unixExpirySpan.Length..(unixExpirySpan.Length + rawTextAsSpan.Length)]);

        Span<byte> destination = stackalloc byte[100];
        unixExpirySpan.CopyTo(destination[..unixExpirySpan.Length]); // copying the expiry bytes to destination
        var total = 8;
        using var hasher = new HMACSHA1(key);

        // hashing the captcha text to hide the text
        hasher.TryComputeHash(rawTextAsSpan, destination[8..], out var totalWrite);
        total += totalWrite;

        // making signature by hashing all the combined bytes
        hasher.TryComputeHash(combined, destination[total..], out totalWrite);
        total += totalWrite;

        var output = destination[..total]; // hash contains timestamp , hashed captcha_text and hash of both. 

        var cipherAsBase64 = Convert.ToBase64String(output);
        return cipherAsBase64;
    }

    public bool Validate(string originalText, string hash)
    {
        // original_text = user sent input

        Span<byte> hashSource = stackalloc byte[100];
        Convert.TryFromBase64String(hash, hashSource, out var totalWrite);

        // first 8 bytes - unix_timestamp
        var timestampSource = hashSource[..8];
        var timestampLong = BitConverter.ToInt64(timestampSource);
        var timestamp = DateTimeOffset.FromUnixTimeSeconds(timestampLong);

        if (timestamp < DateTimeOffset.UtcNow) return false;

        // validate originalText matches with captcha_hash

        Span<byte> originalTextHashed = stackalloc byte[20];
        Span<byte> textAsBytes = stackalloc byte[originalText.Length];
        Encoding.UTF8.GetBytes(originalText, textAsBytes);

        using var hasher = new HMACSHA1(key);
        hasher.TryComputeHash(textAsBytes, originalTextHashed, out _);

        if (!CompareSpan(originalTextHashed, hashSource[8..28]))
        {
            return false;
        }

        // here validate signature
        // validate the hash by combining
        Span<byte> combined = stackalloc byte[textAsBytes.Length + timestampSource.Length];
        timestampSource.CopyTo(combined[..8]);
        textAsBytes.CopyTo(combined[8..(8 + textAsBytes.Length)]);

        // now hash the combined
        Span<byte> combinedHash = stackalloc byte[20];
        hasher.TryComputeHash(combined, combinedHash, out _);
        var signatureFromSource = hashSource[28..totalWrite];
        // compare combinedHashFromSource and combinedHash
        return CompareSpan(combinedHash, signatureFromSource);
    }

    private bool CompareSpan(in Span<byte> arr1, in Span<byte> arr2)
    {
        if (arr1.Length != arr2.Length) return false;

        for (var i = 0; i < arr1.Length; i++)
        {
            if (arr1[i] != arr2[i])
            {
                return false;
            }
        }

        return true;
    }
}