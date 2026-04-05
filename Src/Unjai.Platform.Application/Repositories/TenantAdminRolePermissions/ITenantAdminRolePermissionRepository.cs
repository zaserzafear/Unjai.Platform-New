using Unjai.Platform.Domain.Entities.TenantsAdminRolePermission;

namespace Unjai.Platform.Application.Repositories.TenantAdminRolePermissions;

public interface ITenantAdminRolePermissionRepository
{
    Task<IEnumerable<TenantAdminRolePermission>> GetByRoleId(int roleId, CancellationToken ct = default);
}
