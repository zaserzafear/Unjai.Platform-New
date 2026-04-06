using Unjai.Platform.Api.Endpoints.Extensions;
using Unjai.Platform.Api.Models;
using Unjai.Platform.Application.Services.Tenants.CreateTenant;
using Unjai.Platform.Contracts.Tenants;
using Unjai.Platform.Domain.Entities.TenantsAdminPermission;
using Unjai.Platform.Infrastructure.RateLimiting.Core;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;

namespace Unjai.Platform.Api.Endpoints.Tenants.V1;

public sealed class TenantCreateEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(TenantEndpoints.Base)
            .WithTags(TenantEndpoints.Tag);

        group.MapPost("", async (
            TenantCreateRequestDto request,
            CreateTenantV1 useCase,
            CancellationToken ct) =>
        {
            var result = await useCase.Handle(request, ct);

            return ApiResponseResults.ToHttpResult(result);
        })
            .RequireAuthorization(TenantAdminPermissionCode.CreateTenants.ToString().ToUpperInvariant())
            .EnforceRateLimit(RateLimitPolicyKeys.Default)
            .MapToApiVersion(1);
    }
}
