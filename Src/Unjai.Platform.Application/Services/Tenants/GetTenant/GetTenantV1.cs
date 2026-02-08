using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.Tenants.Dtos;

namespace Unjai.Platform.Application.Services.Tenants.GetTenant;

public interface IGetTenantV1
{
    Task<AppResult<IReadOnlyList<GetTenantResponseDto>>> Handle(int page, int pageSize, CancellationToken cancellationToken);
}

internal sealed class GetTenantV1(ILogger<GetTenantV1> logger, ITenantRepository repository) : IGetTenantV1
{
    public async Task<AppResult<IReadOnlyList<GetTenantResponseDto>>> Handle(int page, int pageSize, CancellationToken cancellationToken)
    {
        if (pageSize is < 1 or > 100)
        {
            return AppResult<IReadOnlyList<GetTenantResponseDto>>.Fail(
                    httpStatus: 400,
                    statusCode: "INVALID_PAGE_SIZE",
                    message: "pageSize must be between 1 and 100"
            );
        }

        if (page < 1)
        {
            return AppResult<IReadOnlyList<GetTenantResponseDto>>.Fail(
                    httpStatus: 400,
                    statusCode: "INVALID_PAGE",
                    message: "page must be greater than 0"
            );
        }

        try
        {
            var result = await repository.GetAll(page, pageSize, cancellationToken);

            if (result.Count == 0)
            {
                return AppResult<IReadOnlyList<GetTenantResponseDto>>.Fail(
                    httpStatus: 404,
                    statusCode: "TENANTS_NOT_FOUND",
                    message: "No tenants found."
                );
            }
            else
            {
                var response = result.Select(t => new GetTenantResponseDto(
                    t.Id,
                    t.Code,
                    t.Name,
                    t.IsActive,
                    t.CreatedAt,
                    t.UpdatedAt
                )).ToList();

                return AppResult<IReadOnlyList<GetTenantResponseDto>>.Ok(
                    httpStatus: 200,
                    statusCode: "TENANTS_FETCHED",
                    message: "Tenants retrieved successfully.",
                    data: response
                );
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while retrieving tenants.");

            return AppResult<IReadOnlyList<GetTenantResponseDto>>.Fail(
                httpStatus: 500,
                statusCode: "INTERNAL_SERVER_ERROR",
                message: ex.Message
            );
        }
    }
}
