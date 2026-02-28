using Unjai.Platform.Api.Endpoints.Extensions;
using Unjai.Platform.Api.Models;
using Unjai.Platform.Application.Services.Tenants.DeleteTenant;
using Unjai.Platform.Domain.Entities.TenantsAdminPermission;
using Unjai.Platform.Infrastructure.RateLimiting.Core;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;

namespace Unjai.Platform.Api.Endpoints.Tenants;

public sealed class DeleteTenantEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(TenantEndpoints.Base)
            .WithTags(TenantEndpoints.Tag);

        group.MapDelete("{id:guid}", async (
            Guid id,
            DeleteTenantV1 useCase,
            CancellationToken ct) =>
        {
            var result = await useCase.Handle(id, ct);

            return ApiResponseResults.ToHttpResult(result);
        })
            .RequireAuthorization(TenantAdminPermissionCode.DeleteTenants.ToString().ToUpperInvariant())
            .EnforceRateLimit(RateLimitPolicyKeys.Default)
            .MapToApiVersion(1);
    }
}
