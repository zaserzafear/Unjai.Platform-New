using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;
using Unjai.Platform.Infrastructure.RateLimiting.Core;

namespace Unjai.Platform.Infrastructure.RateLimiting.Policies;

internal sealed class PolicyResolver(
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
