using Unjai.Platform.Domain.Entities.TenantsAdminRolePermission;

namespace Unjai.Platform.Domain.Entities.TenantsAdminRole;

public sealed class TenantAdminRole
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;

    public ICollection<TenantAdminRolePermission> RolePermissions { get; } = new List<TenantAdminRolePermission>();
}
