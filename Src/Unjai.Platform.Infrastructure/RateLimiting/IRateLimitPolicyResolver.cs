namespace Unjai.Platform.Infrastructure.RateLimiting;

public interface IRateLimitPolicyResolver
{
    RateLimitPolicy Resolve(string policyName);
}
