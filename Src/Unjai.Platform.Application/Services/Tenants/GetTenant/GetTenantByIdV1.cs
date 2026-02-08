using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.Tenants.Dtos;

namespace Unjai.Platform.Application.Services.Tenants.GetTenant;

public interface IGetTenantByIdV1
{
    Task<AppResult<GetTenantResponseDto>> Handle(Guid id, CancellationToken cancellationToken);
}

internal sealed class GetTenantByIdV1(ILogger<GetTenantByIdV1> logger, ITenantRepository repository) : IGetTenantByIdV1
{
    public async Task<AppResult<GetTenantResponseDto>> Handle(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var result = await repository.GetByIdAsync(id, cancellationToken);

            if (result is null)
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
                    result.Id,
                    result.Code,
                    result.Name,
                    result.IsActive,
                    result.CreatedAt,
                    result.UpdatedAt
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
