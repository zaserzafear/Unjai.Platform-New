using Unjai.Platform.Api.Endpoints.Extensions;
using Unjai.Platform.Api.Models;
using Unjai.Platform.Application.Services.Tenants.GetTenant;
using Unjai.Platform.Infrastructure.RateLimiting.Core;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;

namespace Unjai.Platform.Api.Endpoints.Tenants;

public sealed class GetTenantEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(TenantEndpoints.Base)
            .WithTags(TenantEndpoints.Tag);

        group.MapGet("", async (
            int? page,
            int? pageSize,
            IGetTenantAllV1 useCase,
            CancellationToken ct) =>
        {
            var safePage = page.GetValueOrDefault(1);
            var safePageSize = pageSize.GetValueOrDefault(10);

            var result = await useCase.Handle(safePage, safePageSize, ct);

            return ApiResponseResults.ToHttpResult(result);
        })
        .EnforceRateLimit(RateLimitPolicyKeys.Default)
        .MapToApiVersion(1);

        group.MapGet("{id:guid}", async (
            Guid id,
            IGetTenantByIdV1 useCase,
            CancellationToken ct) =>
        {
            var result = await useCase.Handle(id, ct);

            return ApiResponseResults.ToHttpResult(result);
        })
        .EnforceRateLimit(RateLimitPolicyKeys.Default)
        .MapToApiVersion(1);
    }
}
