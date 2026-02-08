using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.Tenants.Dtos;

namespace Unjai.Platform.Application.Services.Tenants.GetTenant;

public interface IGetTenantAllV1
{
    Task<AppResult<IReadOnlyList<GetTenantResponseDto>>> Handle(int page, int pageSize, CancellationToken ct);
}

internal sealed class GetTenantAllV1(
    ILogger<GetTenantAllV1> logger,
    ITenantRepository repository
    ) : IGetTenantAllV1
{
    public async Task<AppResult<IReadOnlyList<GetTenantResponseDto>>> Handle(int page, int pageSize, CancellationToken ct)
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
            var result = await repository.GetAllAsync(page, pageSize, ct);

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
            logger.LogError(
                ex,
                "An error occurred while retrieving tenants. Page: {Page}, PageSize: {PageSize}",
                page,
                pageSize);

            return AppResult<IReadOnlyList<GetTenantResponseDto>>.Fail(
                httpStatus: 500,
                statusCode: "INTERNAL_SERVER_ERROR",
                message: "An unexpected error occurred while retrieving tenants."
            );
        }
    }
}
