using System.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Diagnostics;
using Unjai.Platform.Application.Repositories.TenantAdminRolePermissions;
using Unjai.Platform.Domain.Entities.TenantsAdminRolePermission;

namespace Unjai.Platform.Application.Services.TenantAdminRolePermissions;

public sealed class TenantAdminRolePermissionService(
    ILogger<TenantAdminRolePermissionService> logger,
    ITenantAdminRolePermissionRepository repository,
    HybridCache cache,
    ActivitySource activitySource)
{
    public async Task<IEnumerable<TenantAdminRolePermission>> GetByRoleId(int roleId, CancellationToken ct)
    {
        using var activity = activitySource.StartMethodActivity(typeof(TenantAdminRolePermissionService));

        activity?.SetTag("service", nameof(TenantAdminRolePermissionService));
        activity?.SetTag("operation", nameof(GetByRoleId));
        activity?.SetTag("tenant.admin.role.id", roleId);

        try
        {
            var cacheKey = TenantAdminRolePermissionCacheKeys.GetByRoleId(roleId);
            var cacheHit = true;

            activity?.SetTag("cache.key", cacheKey);

            var permissions = await cache.GetOrCreateAsync(
                cacheKey,
                async innerCt =>
                {
                    cacheHit = false;
                    return await repository.GetByRoleId(roleId, innerCt);
                },
                cancellationToken: ct);

            var permissionsArray = permissions as TenantAdminRolePermission[] ?? permissions.ToArray();

            activity?.SetTag("cache.hit", cacheHit);
            activity?.SetTag("tenant.admin.role.permissions.count", permissionsArray.Length);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return permissionsArray;
        }
        catch (Exception ex)
        {
            activity?.SetTag("error", true);
            activity?.SetTag("error.type", ex.GetType().FullName);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            logger.LogError(
                ex,
                "Failed to retrieve tenant admin role permissions for RoleId {RoleId}.",
                roleId);

            throw;
        }
    }
}
