namespace Unjai.Platform.Infrastructure.RateLimiting;

internal sealed record RateLimitResult(
    bool IsAllowed,
    RateLimitPolicy? Policy)
{
    public static RateLimitResult Allowed()
        => new(true, null);

    public static RateLimitResult Rejected(RateLimitPolicy policy)
        => new(false, policy);
}
