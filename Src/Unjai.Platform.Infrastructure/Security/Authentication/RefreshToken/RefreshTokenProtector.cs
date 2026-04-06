using System.Security.Cryptography;
using System.Text;

namespace Unjai.Platform.Infrastructure.Security.Authentication.RefreshToken;

public static class RefreshTokenProtector
{
    public static string GenerateToken()
    {
        var bytes = RandomNumberGenerator.GetBytes(64);
        return Convert.ToBase64String(bytes);
    }

    public static string HashToken(string token)
    {
        var bytes = Encoding.UTF8.GetBytes(token);
        var hash = SHA256.HashData(bytes);

        return Convert.ToHexString(hash);
    }

    public static bool Verify(string token, string tokenHash)
    {
        var computedHash = HashToken(token);

        return CryptographicOperations.FixedTimeEquals(
            Encoding.UTF8.GetBytes(computedHash),
            Encoding.UTF8.GetBytes(tokenHash));
    }
}
