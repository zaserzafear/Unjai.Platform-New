using Unjai.Platform.Api.Endpoints.Extensions;
using Unjai.Platform.Application.Models;
using Unjai.Platform.Infrastructure.RateLimiting.Core;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;

namespace Unjai.Platform.Api.Endpoints.RateLimiting;

public sealed class RateLimitTestEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(RateLimitingEndpoints.Base)
            .WithTags(RateLimitingEndpoints.Tag);

        group.MapGet("default", (CancellationToken cancellationToken) =>
        {
            var result = AppResult<string>.Ok(
                data: "This is a response from a rate-limited endpoint",
                message: "OK"
            );

            return ApiResponseResults.ToHttpResult(result);
        })
        .EnforceRateLimit(RateLimitPolicyKeys.Default);

        group.MapGet("get-user", (CancellationToken cancellationToken) =>
        {
            var result = AppResult<string>.Ok(
                data: "This is a response from a rate-limited endpoint",
                message: "OK"
            );

            return ApiResponseResults.ToHttpResult(result);
        })
        .EnforceRateLimit(RateLimitPolicyKeys.GetUser);

        group.MapGet("login", (CancellationToken cancellationToken) =>
        {
            var result = AppResult<string>.Ok(
                data: "This is a response from a rate-limited endpoint",
                message: "OK"
            );

            return ApiResponseResults.ToHttpResult(result);
        })
        .EnforceRateLimit(RateLimitPolicyKeys.Login);

        group.MapGet("otp", (CancellationToken cancellationToken) =>
        {
            var result = AppResult<string>.Ok(
                data: "This is a response from a rate-limited endpoint",
                message: "OK"
            );

            return ApiResponseResults.ToHttpResult(result);
        })
        .EnforceRateLimit(RateLimitPolicyKeys.Otp);
    }
}
