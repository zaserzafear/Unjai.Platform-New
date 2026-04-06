using Unjai.Platform.Api.Endpoints.Extensions;
using Unjai.Platform.Api.Models;
using Unjai.Platform.Application.Services.TenantAdmins.LoginTenantAdmin;
using Unjai.Platform.Contracts.TenantAdmins;
using Unjai.Platform.Infrastructure.RateLimiting.Core;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;

namespace Unjai.Platform.Api.Endpoints.TenantAdmins.V1;

public sealed class TenantAdminLoginEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(TenantAdminEndpoints.Base)
            .WithTags(TenantAdminEndpoints.Tag);

        group.MapPost("login", async (
            TenantAdminLoginRequestDto request,
            LoginTenantAdminV1 useCase,
            CancellationToken ct) =>
        {
            var result = await useCase.Handle(request, ct);

            return ApiResponseResults.ToHttpResult(result);
        })
            .AllowAnonymous()
            .EnforceRateLimit(RateLimitPolicyKeys.Login)
            .MapToApiVersion(1);
    }
}
