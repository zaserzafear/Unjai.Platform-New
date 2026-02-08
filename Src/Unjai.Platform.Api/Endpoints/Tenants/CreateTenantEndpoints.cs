using Unjai.Platform.Api.Endpoints.Extensions;
using Unjai.Platform.Api.Models;
using Unjai.Platform.Application.Services.Tenants.CreateTenant;
using Unjai.Platform.Contracts.Tenants.Dtos;
using Unjai.Platform.Infrastructure.RateLimiting.Core;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;

namespace Unjai.Platform.Api.Endpoints.Tenants;

public sealed class CreateTenantEndpoints : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(TenantEndpoints.Base)
            .WithTags(TenantEndpoints.Tag);

        group.MapPost("", async (
            CreateTenantRequestDto request,
            ICreateTenantV1 useCase,
            CancellationToken ct) =>
        {
            var result = await useCase.Handle(request, ct);

            return ApiResponseResults.ToHttpResult(result);
        })
            .EnforceRateLimit(RateLimitPolicyKeys.Default)
            .MapToApiVersion(1);
    }
}
