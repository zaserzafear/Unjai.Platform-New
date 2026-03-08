using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Abstractions.Caching;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Domain.Abstractions;

namespace Unjai.Platform.Application.Services.Tenants.DeleteTenant;

public sealed class DeleteTenantV1(
    ILogger<DeleteTenantV1> logger,
    IUnitOfWork unitOfWork,
    ITenantRepository repository,
    ICacheInvalidationPublisherService cacheInvalidation)
{
    public async Task<AppResult<object>> Handle(Guid id, CancellationToken ct)
    {
        try
        {
            var entity = await repository.GetByIdAsync(id, ct);
            if (entity is null)
            {
                return AppResult<object>.Fail(
                    httpStatus: 404,
                    statusCode: "TENANT_NOT_FOUND",
                    message: "Tenant not found."
                );
            }

            repository.Remove(entity);
            await unitOfWork.SaveChangesAsync(ct);

            var cacheKey = TenantCacheKeys.GetById(id);
            await cacheInvalidation.NotifyCacheInvalidationAsync(cacheKey);

            return AppResult<object>.Ok(
                httpStatus: 200,
                statusCode: "TENANT_DELETED",
                message: "Tenant deleted successfully."
            );
        }
        catch (Exception ex)
        {
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
