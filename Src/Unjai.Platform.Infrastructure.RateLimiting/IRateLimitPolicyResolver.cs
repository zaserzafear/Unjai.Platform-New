namespace Unjai.Platform.Infrastructure.RateLimiting;

internal interface IRateLimitPolicyResolver
{
    RateLimitPolicy Resolve(string policyName);
}
