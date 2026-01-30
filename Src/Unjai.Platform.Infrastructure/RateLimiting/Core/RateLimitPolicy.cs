namespace Unjai.Platform.Infrastructure.RateLimiting.Core;

internal sealed record RateLimitPolicy(string Name, int Limit, TimeSpan Window);
