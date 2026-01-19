namespace Unjai.Platform.Infrastructure.Messaging.Redis;

internal sealed record RateLimitBlockedMessage(string Key, int TtlSeconds);
