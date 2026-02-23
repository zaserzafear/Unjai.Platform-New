using Unjai.Platform.Api.Endpoints.Extensions;
using Unjai.Platform.Api.Models;
using Unjai.Platform.Application.Services.TenantAdmins.LoginTenantAdmin;
using Unjai.Platform.Contracts.TenantAdmins.Dtos;
using Unjai.Platform.Infrastructure.RateLimiting.Core;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;

namespace Unjai.Platform.Api.Endpoints.TenantAdmins;

public sealed class LoginTenantAdminEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(TenantAdminEndpoints.Base)
            .WithTags(TenantAdminEndpoints.Tag);

        group.MapPost("login", async (
            LoginTenantAdminRequestDto request,
            LoginTenantAdminV1 useCase,
            CancellationToken ct) =>
        {
            var result = await useCase.Handle(request, ct);

            return ApiResponseResults.ToHttpResult(result);
        })
            .EnforceRateLimit(RateLimitPolicyKeys.Login)
            .MapToApiVersion(1);
    }
}
