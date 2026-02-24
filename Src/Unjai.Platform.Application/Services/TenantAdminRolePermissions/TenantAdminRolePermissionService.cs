using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Repositories.TenantAdminRolePermissions;
using Unjai.Platform.Domain.Entities.TenantsAdminRolePermission;

namespace Unjai.Platform.Application.Services.TenantAdminRolePermissions;

public sealed class TenantAdminRolePermissionService(
     ILogger<TenantAdminRolePermissionService> logger,
     ITenantAdminRolePermissionRepository repository,
     HybridCache cache)
{
    public async Task<IEnumerable<TenantAdminRolePermission>> GetByRoleId(int roleId, CancellationToken ct)
    {
        try
        {
            var cacheKey = TenantAdminRolePermissionCacheKeys.GetByRoleId(roleId);
            var permissions = await cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    return await repository.GetByRoleId(roleId, ct);
                }, cancellationToken: ct);
            return permissions;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve tenant admin role permissions for RoleId {RoleId} from the repository.", roleId);
            throw;
        }
    }
}
