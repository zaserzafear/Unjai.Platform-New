using System.Security.Cryptography;

namespace Unjai.Platform.Infrastructure.Security.Helpers;

public static class CryptoHelper
{
    /// <summary>
    /// Generate a cryptographically secure secret suitable for JWT HMAC signing.
    /// Default: 512-bit (64 bytes).
    /// </summary>
    public static string GenerateSecret(int byteLength = 64)
    {
        if (byteLength < 32)
            throw new ArgumentOutOfRangeException(
                nameof(byteLength),
                "Secret must be at least 256 bits (32 bytes).");

        return GenerateBase64(byteLength);
    }

    private static string GenerateBase64(int byteLength)
    {
        var bytes = RandomNumberGenerator.GetBytes(byteLength);
        return Convert.ToBase64String(bytes);
    }
}
