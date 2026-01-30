using Microsoft.AspNetCore.Http;
using Unjai.Platform.Infrastructure.RateLimiting.Context;

namespace Unjai.Platform.Infrastructure.RateLimiting.Extensions;

internal static class HttpContextRateLimitExtensions
{
    private static readonly object Key = new();

    public static void SetRateLimitContext(
        this HttpContext context,
        RateLimitContext value)
    {
        context.Items[Key] = value;
    }

    public static bool TryGetRateLimitContext(
        this HttpContext context,
        out RateLimitContext? value)
    {
        if (context.Items.TryGetValue(Key, out var obj) &&
            obj is RateLimitContext ctx)
        {
            value = ctx;
            return true;
        }

        value = null;
        return false;
    }
}
