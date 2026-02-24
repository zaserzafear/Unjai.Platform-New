namespace Unjai.Platform.Domain.Entities.TenantsAdminRole;

public static class TenantAdminRoleSeed
{
    public static TenantAdminRole[] GetSeedData() =>
    [
        new TenantAdminRole
        {
            Id = (int) TenantAdminRoleCode.SuperAdmin,
            Code = TenantAdminRoleCode.SuperAdmin.ToString().ToUpperInvariant(),
            Name = "Super Admin"
        },
        new TenantAdminRole
        {
            Id = (int)TenantAdminRoleCode.Admin,
            Code = TenantAdminRoleCode.Admin.ToString().ToUpperInvariant(),
            Name = "Admin"
        }
    ];
}
