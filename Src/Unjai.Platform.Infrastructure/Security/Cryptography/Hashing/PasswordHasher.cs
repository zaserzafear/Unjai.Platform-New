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

    public static bool Verify(string plainPassword, string passwordHash) =>
        !string.IsNullOrEmpty(plainPassword) &&
        !string.IsNullOrEmpty(passwordHash) &&
        BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
}
