namespace Unjai.Platform.Infrastructure.RateLimiting.Configurations;

public sealed class RateLimitingOptions
{
    public string Secret { get; set; } = string.Empty;
    public TimeSpan ContextTtl { get; set; }

    public Dictionary<string, RateLimitPolicyOptions> Policies { get; } = new();
}

public sealed class RateLimitPolicyOptions
{
    public int Limit { get; set; }
    public TimeSpan Window { get; set; }
}
