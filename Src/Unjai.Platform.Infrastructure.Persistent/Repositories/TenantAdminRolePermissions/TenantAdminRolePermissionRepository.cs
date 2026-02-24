using Microsoft.EntityFrameworkCore;
using Unjai.Platform.Application.Repositories.TenantAdminRolePermissions;
using Unjai.Platform.Domain.Entities.TenantsAdminRolePermission;
using Unjai.Platform.Infrastructure.Persistent.Database;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.TenantAdminRolePermissions;

internal sealed class TenantAdminRolePermissionRepository(ReadDbContext readDbContext) : ITenantAdminRolePermissionRepository
{
    public async Task<IEnumerable<TenantAdminRolePermission>> GetByRoleId(int roleId, CancellationToken ct)
    {
        return await readDbContext
            .TenantAdminRolePermissions
            .Where(x => x.RoleId == roleId)
            .Select(x => new TenantAdminRolePermission
            {
                RoleId = x.RoleId,
                PermissionId = x.PermissionId,
            })
            .ToListAsync(ct);
    }
}
