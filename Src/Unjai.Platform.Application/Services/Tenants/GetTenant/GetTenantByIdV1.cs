using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.Tenants.Dtos;

namespace Unjai.Platform.Application.Services.Tenants.GetTenant;

public interface IGetTenantByIdV1
{
    Task<AppResult<GetTenantResponseDto>> Handle(Guid id, CancellationToken ct);
}

internal sealed class GetTenantByIdV1(
    ILogger<GetTenantByIdV1> logger,
    ITenantRepository repository,
    HybridCache cache)
    : IGetTenantByIdV1
{
    public async Task<AppResult<GetTenantResponseDto>> Handle(Guid id, CancellationToken ct)
    {
        try
        {
            var cacheKey = TenantCacheKey.GetById(id);

            var tenant = await cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    return await repository.GetByIdAsync(id, ct);
                },
                cancellationToken: ct);

            if (tenant is null)
            {
                return AppResult<GetTenantResponseDto>.Fail(
                    httpStatus: 404,
                    statusCode: "TENANTS_NOT_FOUND",
                    message: "Tenant not found"
                );
            }
            else
            {
                var response = new GetTenantResponseDto(
                    tenant.Id,
                    tenant.Code,
                    tenant.Name,
                    tenant.IsActive,
                    tenant.CreatedAt,
                    tenant.UpdatedAt
                );

                return AppResult<GetTenantResponseDto>.Ok(
                    httpStatus: 200,
                    statusCode: "TENANTS_FETCHED",
                    message: "Tenants retrieved successfully.",
                    data: response
                );
            }
        }
        catch (Exception ex)
        {
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
