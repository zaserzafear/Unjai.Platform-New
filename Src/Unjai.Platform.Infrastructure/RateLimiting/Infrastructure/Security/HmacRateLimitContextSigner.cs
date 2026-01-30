using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;
using Unjai.Platform.Infrastructure.RateLimiting.Context;

namespace Unjai.Platform.Infrastructure.RateLimiting.Infrastructure.Security;

internal sealed class HmacRateLimitContextSigner : IRateLimitContextSigner
{
    private readonly byte[] _secret;
    private readonly TimeSpan _ttl;

    public HmacRateLimitContextSigner(
        byte[] secret,
        TimeSpan ttl)
    {
        _secret = secret;
        _ttl = ttl;
    }

    public string Sign(string policyName)
    {
        var payload = new RateLimitSignedPayload(
            policyName,
            DateTimeOffset.UtcNow.ToUnixTimeSeconds());

        var json = JsonSerializer.Serialize(payload);
        var payloadBytes = Encoding.UTF8.GetBytes(json);

        using var hmac = new HMACSHA256(_secret);
        var signature = hmac.ComputeHash(payloadBytes);

        return $"{Convert.ToBase64String(payloadBytes)}.{Convert.ToBase64String(signature)}";
    }

    public bool TryValidate(string headerValue, string expectedPolicy)
    {
        var parts = headerValue.Split('.');
        if (parts.Length != 2)
            return false;

        var payloadBytes = Convert.FromBase64String(parts[0]);
        var signatureBytes = Convert.FromBase64String(parts[1]);

        using var hmac = new HMACSHA256(_secret);
        var computed = hmac.ComputeHash(payloadBytes);

        if (!CryptographicOperations.FixedTimeEquals(signatureBytes, computed))
            return false;

        var payload = JsonSerializer.Deserialize<RateLimitSignedPayload>(payloadBytes);
        if (payload is null || payload.Policy != expectedPolicy)
            return false;

        var issuedAt = DateTimeOffset.FromUnixTimeSeconds(payload.IssuedAtUnix);
        return DateTimeOffset.UtcNow - issuedAt <= _ttl;
    }
}
