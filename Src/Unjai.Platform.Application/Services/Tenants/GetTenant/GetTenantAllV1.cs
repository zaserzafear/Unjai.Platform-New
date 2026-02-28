using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.Tenants;

namespace Unjai.Platform.Application.Services.Tenants.GetTenant;

public sealed class GetTenantAllV1(
    ILogger<GetTenantAllV1> logger,
    ITenantRepository repository
    )
{
    public async Task<AppResult<PagedResult<GetTenantResponseDto>>> Handle(
        int page,
        int pageSize,
        CancellationToken ct)
    {
        if (pageSize is < 1 or > 100)
        {
            return AppResult<PagedResult<GetTenantResponseDto>>.Fail(
                400, "INVALID_PAGE_SIZE", "pageSize must be between 1 and 100");
        }

        if (page < 1)
        {
            return AppResult<PagedResult<GetTenantResponseDto>>.Fail(
                400, "INVALID_PAGE", "page must be greater than 0");
        }

        try
        {
            var pagedTenants = await repository.GetAllAsync(page, pageSize, ct);

            var dtoItems = pagedTenants.Items
                .Select(t => new GetTenantResponseDto(
                    t.Id,
                    t.Code,
                    t.Name,
                    t.IsActive,
                    t.CreatedAt,
                    t.UpdatedAt))
                .ToList();

            var response = new PagedResult<GetTenantResponseDto>(
                Items: dtoItems,
                Page: pagedTenants.Page,
                PageSize: pagedTenants.PageSize,
                TotalCount: pagedTenants.TotalCount
            );

            return AppResult<PagedResult<GetTenantResponseDto>>.Ok(
                200,
                "TENANTS_FETCHED",
                "Tenants retrieved successfully.",
                response
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while retrieving tenants. Page: {Page}, PageSize: {PageSize}",
                page,
                pageSize);

            return AppResult<PagedResult<GetTenantResponseDto>>.Fail(
                500,
                "INTERNAL_SERVER_ERROR",
                "An unexpected error occurred while retrieving tenants."
            );
        }
    }
}
