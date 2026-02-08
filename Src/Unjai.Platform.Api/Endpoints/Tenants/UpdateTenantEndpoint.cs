using Unjai.Platform.Api.Endpoints.Extensions;
using Unjai.Platform.Api.Models;
using Unjai.Platform.Application.Services.Tenants.UpdateTenant;
using Unjai.Platform.Contracts.Tenants.Dtos;
using Unjai.Platform.Infrastructure.RateLimiting.Core;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;

namespace Unjai.Platform.Api.Endpoints.Tenants;

public sealed class UpdateTenantEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(TenantEndpoints.Base)
            .WithTags(TenantEndpoints.Tag);

        group.MapPut("{id:guid}", async (
            Guid id,
            UpdateTenantRequestDto request,
            IUpdateTenantV1 useCase,
            CancellationToken cancellationToken) =>
        {
            var result = await useCase.Handle(id, request, cancellationToken);

            return ApiResponseResults.ToHttpResult(result);
        })
            .EnforceRateLimit(RateLimitPolicyKeys.Default)
            .MapToApiVersion(1);
    }
}
