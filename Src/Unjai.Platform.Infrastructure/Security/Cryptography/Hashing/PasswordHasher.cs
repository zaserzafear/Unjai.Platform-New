namespace Unjai.Platform.Infrastructure.Security.Cryptography.Hashing;

public static class PasswordHasher
{
    private const int WorkFactor = 11;

    public static string Hash(string plainPassword)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            throw new ArgumentException("Password cannot be empty.", nameof(plainPassword));

        return BCrypt.Net.BCrypt.HashPassword(
            plainPassword,
            workFactor: WorkFactor);
    }

    public static bool Verify(string plainPassword, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(plainPassword))
            return false;

        if (string.IsNullOrWhiteSpace(passwordHash))
            return false;

        return BCrypt.Net.BCrypt.Verify(
            plainPassword,
            passwordHash);
    }
}
