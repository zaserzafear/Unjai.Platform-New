namespace Unjai.Platform.Infrastructure.RateLimiting;

internal sealed class FixedWindowPolicyResolver(
    IReadOnlyDictionary<string, RateLimitPolicy> policies
    ) : IRateLimitPolicyResolver
{
    public RateLimitPolicy Resolve(string policyName)
    {
        if (!policies.TryGetValue(policyName, out var policy))
        {
            throw new InvalidOperationException(
                $"Rate limit policy '{policyName}' is not registered.");
        }

        return policy;
    }
}
