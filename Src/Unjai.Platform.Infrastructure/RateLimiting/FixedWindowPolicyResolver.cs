using System.Globalization;

namespace Unjai.Platform.Infrastructure.RateLimiting;

public sealed class FixedWindowPolicyResolver : IRateLimitPolicyResolver
{
    public RateLimitPolicy Resolve(string policyName)
    {
        var parts = policyName.Split('-', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 4 || parts[0] != "fixed")
            throw new InvalidOperationException($"Invalid policy: {policyName}");

        var limit = int.Parse(parts[1], CultureInfo.InvariantCulture);

        TimeSpan window = parts[3].Contains(':')
            ? TimeSpan.Parse(parts[3], CultureInfo.InvariantCulture)
            : parts[3] switch
            {
                "seconds" => TimeSpan.FromSeconds(1),
                "minutes" => TimeSpan.FromMinutes(1),
                "hours" => TimeSpan.FromHours(1),
                "days" => TimeSpan.FromDays(1),
                _ => throw new InvalidOperationException($"Unsupported window: {parts[3]}")
            };

        return new RateLimitPolicy(policyName, limit, window);
    }
}
