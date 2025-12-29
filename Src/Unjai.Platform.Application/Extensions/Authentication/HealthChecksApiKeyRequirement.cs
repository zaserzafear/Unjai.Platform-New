using Microsoft.AspNetCore.Authorization;

namespace Unjai.Platform.Application.Extensions.Authentication;

internal sealed class HealthChecksApiKeyRequirement : IAuthorizationRequirement
{
    public string ApiKey { get; }

    public HealthChecksApiKeyRequirement(string apiKey)
    {
        ApiKey = apiKey;
    }
}
