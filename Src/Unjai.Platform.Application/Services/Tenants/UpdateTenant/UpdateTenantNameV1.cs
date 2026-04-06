using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Abstractions.Caching;
using Unjai.Platform.Application.Diagnostics;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.Tenants;
using Unjai.Platform.Domain.Abstractions;
using Unjai.Platform.Domain.Entities.Tenants;

namespace Unjai.Platform.Application.Services.Tenants.UpdateTenant;

public sealed class UpdateTenantNameV1(
    ILogger<UpdateTenantNameV1> logger,
    IUnitOfWork unitOfWork,
    ITenantRepository repository,
    ICacheInvalidationPublisherService cacheInvalidation,
    ActivitySource activitySource)
{
    public async Task<AppResult<object>> Handle(Guid id, TenantUpdateNameRequestDto request, CancellationToken ct)
    {
        using var activity = activitySource.StartMethodActivity(typeof(UpdateTenantNameV1));

        activity?.SetTag("service", nameof(UpdateTenantNameV1));
        activity?.SetTag("operation", nameof(Handle));
        activity?.SetTag("tenant.id", id);

        try
        {
            Tenant? entity;

            using var getActivity = activitySource.StartActivity("tenant.get_by_id");
            try
            {
                getActivity?.SetTag("service", nameof(UpdateTenantNameV1));
                getActivity?.SetTag("operation", "GetByIdAsync");
                getActivity?.SetTag("tenant.id", id);

                entity = await repository.GetByIdAsync(id, ct);

                getActivity?.SetTag("tenant.exists", entity is not null);
                getActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                getActivity?.SetTag("error", true);
                getActivity?.SetTag("error.type", ex.GetType().FullName);
                getActivity?.SetTag("error.message", ex.Message);
                getActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }

            if (entity is null)
            {
                activity?.SetTag("tenant.update.result", "not_found");
                activity?.SetStatus(ActivityStatusCode.Ok);

                return AppResult<object>.Fail(
                    httpStatus: 404,
                    statusCode: "TENANT_NOT_FOUND",
                    message: "Tenant not found."
                );
            }

            entity.SetName(request.Name);

            using var updateActivity = activitySource.StartActivity("tenant.update");
            try
            {
                updateActivity?.SetTag("service", nameof(UpdateTenantNameV1));
                updateActivity?.SetTag("operation", "UpdateTenantName");
                updateActivity?.SetTag("tenant.id", id);

                repository.Update(entity);
                await unitOfWork.SaveChangesAsync(ct);

                updateActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                updateActivity?.SetTag("error", true);
                updateActivity?.SetTag("error.type", ex.GetType().FullName);
                updateActivity?.SetTag("error.message", ex.Message);
                updateActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }

            var cacheKey = TenantCacheKeys.GetById(id);

            using var cacheActivity = activitySource.StartActivity("tenant.cache.invalidate");
            try
            {
                cacheActivity?.SetTag("service", nameof(UpdateTenantNameV1));
                cacheActivity?.SetTag("operation", "CacheInvalidation");
                cacheActivity?.SetTag("cache.key", cacheKey);

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

            activity?.SetTag("tenant.update.result", "success");
            activity?.SetStatus(ActivityStatusCode.Ok);

            return AppResult<object>.Ok(
                httpStatus: 200,
                statusCode: "TENANT_UPDATED",
                message: "Tenant updated successfully."
            );
        }
        catch (Exception ex)
        {
            activity?.SetTag("tenant.update.result", "error");
            activity?.SetTag("error", true);
            activity?.SetTag("error.type", ex.GetType().FullName);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            logger.LogError(
                ex,
                "An error occurred while updating tenant with id '{TenantId}'",
                id);

            return AppResult<object>.Fail(
                httpStatus: 500,
                statusCode: "INTERNAL_SERVER_ERROR",
                message: "An unexpected error occurred while updating tenant."
            );
        }
    }
}
