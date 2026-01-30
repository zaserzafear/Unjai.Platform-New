namespace Unjai.Platform.Infrastructure.RateLimiting.Context;

internal sealed record RateLimitContext(
    string PolicyName,
    string Signature,
    DateTimeOffset IssuedAt);
