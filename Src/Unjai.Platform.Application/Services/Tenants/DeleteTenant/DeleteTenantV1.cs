using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Abstractions.Caching;
using Unjai.Platform.Application.Diagnostics;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Domain.Abstractions;
using Unjai.Platform.Domain.Entities.Tenants;

namespace Unjai.Platform.Application.Services.Tenants.DeleteTenant;

public sealed class DeleteTenantV1(
    ILogger<DeleteTenantV1> logger,
    IUnitOfWork unitOfWork,
    ITenantRepository repository,
    ICacheInvalidationPublisherService cacheInvalidation,
    ActivitySource activitySource)
{
    public async Task<AppResult<object>> Handle(Guid id, CancellationToken ct)
    {
        using var activity = activitySource.StartMethodActivity(typeof(DeleteTenantV1));

        activity?.SetTag("service", nameof(DeleteTenantV1));
        activity?.SetTag("operation", nameof(Handle));
        activity?.SetTag("tenant.id", id);

        try
        {
            Tenant? entity;

            using (var dbActivity = activitySource.StartActivity("tenant.get_by_id"))
            {
                dbActivity?.SetTag("service", nameof(DeleteTenantV1));
                dbActivity?.SetTag("operation", "GetByIdAsync");
                dbActivity?.SetTag("tenant.id", id);

                try
                {
                    entity = await repository.GetByIdAsync(id, ct);

                    dbActivity?.SetTag("tenant.exists", entity is not null);
                    dbActivity?.SetStatus(ActivityStatusCode.Ok);
                }
                catch (Exception ex)
                {
                    dbActivity?.SetTag("error", true);
                    dbActivity?.SetTag("error.type", ex.GetType().FullName);
                    dbActivity?.SetTag("error.message", ex.Message);
                    dbActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    throw;
                }
            }

            if (entity is null)
            {
                activity?.SetTag("tenant.delete.result", "not_found");
                activity?.SetStatus(ActivityStatusCode.Ok);

                return AppResult<object>.Fail(
                    httpStatus: 404,
                    statusCode: "TENANT_NOT_FOUND",
                    message: "Tenant not found."
                );
            }

            using (var dbActivity = activitySource.StartActivity("tenant.delete"))
            {
                dbActivity?.SetTag("service", nameof(DeleteTenantV1));
                dbActivity?.SetTag("operation", "RemoveTenant");
                dbActivity?.SetTag("tenant.id", id);

                try
                {
                    repository.Remove(entity);
                    await unitOfWork.SaveChangesAsync(ct);

                    dbActivity?.SetStatus(ActivityStatusCode.Ok);
                }
                catch (Exception ex)
                {
                    dbActivity?.SetTag("error", true);
                    dbActivity?.SetTag("error.type", ex.GetType().FullName);
                    dbActivity?.SetTag("error.message", ex.Message);
                    dbActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    throw;
                }
            }

            var cacheKey = TenantCacheKeys.GetById(id);

            using (var cacheActivity = activitySource.StartActivity("tenant.cache.invalidate"))
            {
                cacheActivity?.SetTag("service", nameof(DeleteTenantV1));
                cacheActivity?.SetTag("operation", "CacheInvalidation");
                cacheActivity?.SetTag("cache.key", cacheKey);

                try
                {
                    await cacheInvalidation.NotifyCacheInvalidationAsync(cacheKey);

                    cacheActivity?.SetStatus(ActivityStatusCode.Ok);
                }
                catch (Exception ex)
                {
                    cacheActivity?.SetTag("error", true);
                    cacheActivity?.SetTag("error.type", ex.GetType().FullName);
                    cacheActivity?.SetTag("error.message", ex.Message);
                    cacheActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                    throw;
                }
            }

            activity?.SetTag("tenant.delete.result", "success");
            activity?.SetStatus(ActivityStatusCode.Ok);

            return AppResult<object>.Ok(
                httpStatus: 200,
                statusCode: "TENANT_DELETED",
                message: "Tenant deleted successfully."
            );
        }
        catch (Exception ex)
        {
            activity?.SetTag("tenant.delete.result", "error");
            activity?.SetTag("error", true);
            activity?.SetTag("error.type", ex.GetType().FullName);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            logger.LogError(
                ex,
                "An error occurred while deleting tenant with id '{TenantId}'",
                id);

            return AppResult<object>.Fail(
                httpStatus: 500,
                statusCode: "INTERNAL_SERVER_ERROR",
                message: "An unexpected error occurred while deleting tenant."
            );
        }
    }
}
