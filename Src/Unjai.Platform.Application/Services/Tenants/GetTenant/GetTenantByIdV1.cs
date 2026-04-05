using System.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Diagnostics;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.Tenants;

namespace Unjai.Platform.Application.Services.Tenants.GetTenant;

public sealed class GetTenantByIdV1(
    ILogger<GetTenantByIdV1> logger,
    ITenantRepository repository,
    HybridCache cache,
    ActivitySource activitySource)
{
    public async Task<AppResult<GetTenantResponseDto>> Handle(Guid id, CancellationToken ct)
    {
        using var activity = activitySource.StartMethodActivity(typeof(GetTenantByIdV1));

        activity?.SetTag("service", nameof(GetTenantByIdV1));
        activity?.SetTag("operation", nameof(Handle));
        activity?.SetTag("tenant.id", id);

        try
        {
            var cacheKey = TenantCacheKeys.GetById(id);
            var cacheHit = true;

            activity?.SetTag("cache.key", cacheKey);

            var tenant = await cache.GetOrCreateAsync(
                cacheKey,
                async innerCt =>
                {
                    cacheHit = false;

                    using var repoActivity = activitySource.StartActivity("tenant.get_by_id");
                    try
                    {
                        repoActivity?.SetTag("service", nameof(GetTenantByIdV1));
                        repoActivity?.SetTag("operation", "GetByIdAsync");
                        repoActivity?.SetTag("tenant.id", id);

                        var result = await repository.GetByIdAsync(id, innerCt);

                        repoActivity?.SetTag("tenant.exists", result is not null);
                        repoActivity?.SetStatus(ActivityStatusCode.Ok);

                        return result;
                    }
                    catch (Exception ex)
                    {
                        repoActivity?.SetTag("error", true);
                        repoActivity?.SetTag("error.type", ex.GetType().FullName);
                        repoActivity?.SetTag("error.message", ex.Message);
                        repoActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                        throw;
                    }
                },
                cancellationToken: ct);

            activity?.SetTag("cache.hit", cacheHit);
            activity?.SetTag("tenant.exists", tenant is not null);

            if (tenant is null)
            {
                activity?.SetTag("tenant.fetch.result", "not_found");
                activity?.SetStatus(ActivityStatusCode.Ok);

                return AppResult<GetTenantResponseDto>.Fail(
                    httpStatus: 404,
                    statusCode: "TENANTS_NOT_FOUND",
                    message: "Tenant not found"
                );
            }

            var response = new GetTenantResponseDto(
                tenant.Id,
                tenant.Code,
                tenant.Name,
                tenant.IsActive,
                tenant.CreatedAt,
                tenant.UpdatedAt
            );

            activity?.SetTag("tenant.fetch.result", "success");
            activity?.SetStatus(ActivityStatusCode.Ok);

            return AppResult<GetTenantResponseDto>.Ok(
                httpStatus: 200,
                statusCode: "TENANTS_FETCHED",
                message: "Tenants retrieved successfully.",
                data: response
            );
        }
        catch (Exception ex)
        {
            activity?.SetTag("tenant.fetch.result", "error");
            activity?.SetTag("error", true);
            activity?.SetTag("error.type", ex.GetType().FullName);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            logger.LogError(
                ex,
                "An error occurred while retrieving tenant with id '{TenantId}'",
                id);

            return AppResult<GetTenantResponseDto>.Fail(
                httpStatus: 500,
                statusCode: "INTERNAL_SERVER_ERROR",
                message: "An unexpected error occurred while retrieving tenant."
            );
        }
    }
}
