namespace Unjai.Platform.Infrastructure.RateLimiting;

public sealed record RateLimitPolicy(string Name, int Limit, TimeSpan Window);
