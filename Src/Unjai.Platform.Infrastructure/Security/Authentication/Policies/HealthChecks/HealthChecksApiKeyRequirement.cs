using Microsoft.AspNetCore.Authorization;

namespace Unjai.Platform.Infrastructure.Security.Authentication.Policies.HealthChecks;

internal sealed class HealthChecksApiKeyRequirement : IAuthorizationRequirement
{
    public string ApiKey { get; }

    public HealthChecksApiKeyRequirement(string apiKey)
    {
        ApiKey = apiKey;
    }
}
