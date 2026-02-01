using Unjai.Platform.Infrastructure.RateLimiting.Configurations;

namespace Unjai.Platform.Infrastructure.RateLimiting.Core;

internal sealed record RateLimitPolicy(
    string Name,
    RateLimitStrategy Strategy,
    int Limit,
    TimeSpan Window,
    int? TokensPerPeriod,
    TimeSpan? ReplenishmentPeriod);
