using Unjai.Platform.Domain.Entities.TenantsAdminPermission;
using Unjai.Platform.Domain.Entities.TenantsAdminRole;

namespace Unjai.Platform.Domain.Entities.TenantsAdminRolePermission;

public sealed class TenantAdminRolePermission
{
    public int RoleId { get; set; }
    public TenantAdminRole Role { get; set; } = null!;

    public int PermissionId { get; set; }
    public TenantAdminPermission Permission { get; set; } = null!;
}
