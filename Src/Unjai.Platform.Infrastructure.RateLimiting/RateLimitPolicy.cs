namespace Unjai.Platform.Infrastructure.RateLimiting;

internal sealed record RateLimitPolicy(string Name, int Limit, TimeSpan Window);
