using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Diagnostics;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.Tenants;

namespace Unjai.Platform.Application.Services.Tenants.GetTenant;

public sealed class GetTenantAllV1(
    ILogger<GetTenantAllV1> logger,
    ITenantRepository repository,
    ActivitySource activitySource)
{
    public async Task<AppResult<PagedResult<GetTenantResponseDto>>> Handle(
    int page,
    int pageSize,
    CancellationToken ct)
    {
        using var activity = activitySource.StartMethodActivity(typeof(GetTenantAllV1));

        activity?.SetTag("service", nameof(GetTenantAllV1));
        activity?.SetTag("operation", nameof(Handle));
        activity?.SetTag("pagination.page", page);
        activity?.SetTag("pagination.page_size", pageSize);

        try
        {
            if (pageSize is < 1 or > 100)
            {
                activity?.SetTag("validation.result", "failed");
                activity?.SetTag("validation.error.code", "INVALID_PAGE_SIZE");
                activity?.SetStatus(ActivityStatusCode.Ok);

                return AppResult<PagedResult<GetTenantResponseDto>>.Fail(
                    400,
                    "INVALID_PAGE_SIZE",
                    "pageSize must be between 1 and 100");
            }

            if (page < 1)
            {
                activity?.SetTag("validation.result", "failed");
                activity?.SetTag("validation.error.code", "INVALID_PAGE");
                activity?.SetStatus(ActivityStatusCode.Ok);

                return AppResult<PagedResult<GetTenantResponseDto>>.Fail(
                    400,
                    "INVALID_PAGE",
                    "page must be greater than 0");
            }

            using var repositoryActivity = activitySource.StartActivity("tenant.get_all");
            PagedResult<Unjai.Platform.Domain.Entities.Tenants.Tenant> pagedTenants;

            try
            {
                repositoryActivity?.SetTag("service", nameof(GetTenantAllV1));
                repositoryActivity?.SetTag("operation", "GetAllAsync");
                repositoryActivity?.SetTag("pagination.page", page);
                repositoryActivity?.SetTag("pagination.page_size", pageSize);

                pagedTenants = await repository.GetAllAsync(page, pageSize, ct);

                repositoryActivity?.SetTag("tenant.items.count", pagedTenants.Items.Count);
                repositoryActivity?.SetTag("tenant.total_count", pagedTenants.TotalCount);
                repositoryActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                repositoryActivity?.SetTag("error", true);
                repositoryActivity?.SetTag("error.type", ex.GetType().FullName);
                repositoryActivity?.SetTag("error.message", ex.Message);
                repositoryActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }

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

            activity?.SetTag("tenant.items.count", dtoItems.Count);
            activity?.SetTag("tenant.total_count", pagedTenants.TotalCount);
            activity?.SetTag("tenant.fetch.result", "success");
            activity?.SetStatus(ActivityStatusCode.Ok);

            return AppResult<PagedResult<GetTenantResponseDto>>.Ok(
                200,
                "TENANTS_FETCHED",
                "Tenants retrieved successfully.",
                response
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
