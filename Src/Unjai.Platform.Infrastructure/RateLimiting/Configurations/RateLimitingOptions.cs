namespace Unjai.Platform.Infrastructure.RateLimiting.Configurations;

public sealed class RateLimitingOptions
{
    public Dictionary<string, RateLimitPolicyOptions> Policies { get; init; }
        = new();
}

public sealed class RateLimitPolicyOptions
{
    public int Limit { get; init; }
    public TimeSpan Window { get; init; }
}
