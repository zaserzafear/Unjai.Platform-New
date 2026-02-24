namespace Unjai.Platform.Infrastructure.RateLimiting.Context;

internal sealed record RateLimitBlockedMessage(string Key, int TtlSeconds);
