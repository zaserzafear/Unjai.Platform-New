using Unjai.Platform.Api.Models;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;

namespace Unjai.Platform.Api.RateLimiting;

public sealed class RateLimitResultFactory
    : IMinimalRateLimitResultFactory
{
    public IResult CreateTooManyRequestsResult(HttpContext httpContext)
    {
        var appResult = AppResult<object>.Fail(
            httpStatus: StatusCodes.Status429TooManyRequests,
            statusCode: "RATE_LIMIT_EXCEEDED",
            message: "Too many requests. Please try again later."
        );

        return ApiResponseResults.ToHttpResult(appResult);
    }
}
