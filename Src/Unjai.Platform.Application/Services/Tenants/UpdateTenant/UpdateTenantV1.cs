using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.Tenants.Dtos;
using Unjai.Platform.Domain.Abstractions;
using Unjai.Platform.Infrastructure.Caching.Services;

namespace Unjai.Platform.Application.Services.Tenants.UpdateTenant;

public interface IUpdateTenantV1
{
    public Task<AppResult<object>> Handle(Guid id, UpdateTenantRequestDto request, CancellationToken ct);
}

internal sealed class UpdateTenantV1(
    ILogger<UpdateTenantV1> logger,
    IUnitOfWork unitOfWork,
    ITenantRepository repository,
    ICacheInvalidationPublisherService cacheInvalidation)
    : IUpdateTenantV1
{
    public async Task<AppResult<object>> Handle(Guid id, UpdateTenantRequestDto request, CancellationToken ct)
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

            entity.Name = request.Name;

            repository.Update(entity);
            await unitOfWork.SaveChangesAsync(ct);

            var cacheKey = TenantCacheKey.GetById(id);
            await cacheInvalidation.NotifyCacheInvalidationAsync(cacheKey);

            return AppResult<object>.Ok(
                httpStatus: 200,
                statusCode: "TENANT_UPDATED",
                message: "Tenant updated successfully."
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
