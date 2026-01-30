namespace Unjai.Platform.Infrastructure.RateLimiting.Context;

internal sealed record RateLimitSignedPayload(
    string Policy,
    long IssuedAtUnix);
