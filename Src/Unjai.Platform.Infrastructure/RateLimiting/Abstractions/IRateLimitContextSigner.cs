namespace Unjai.Platform.Infrastructure.RateLimiting.Abstractions;

public interface IRateLimitContextSigner
{
    string Sign(string policyName);
    bool TryValidate(string headerValue, string expectedPolicy);
}
