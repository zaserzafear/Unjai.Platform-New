using Unjai.Platform.Domain.Entities.TenantsAdminPermission;
using Unjai.Platform.Domain.Entities.TenantsAdminRole;

namespace Unjai.Platform.Domain.Entities.TenantsAdminRolePermission;

public static class TenantAdminRolePermissionSeed
{
    public static TenantAdminRolePermission[] GetSeedData() =>
    [
        new TenantAdminRolePermission
        {
            RoleId = (int)TenantAdminRoleCode.SuperAdmin,
            PermissionId = (int)TenantAdminPermissionCode.ReadTenants
        },
        new TenantAdminRolePermission
        {
            RoleId = (int)TenantAdminRoleCode.SuperAdmin,
            PermissionId = (int)TenantAdminPermissionCode.CreateTenants
        },
        new TenantAdminRolePermission
        {
            RoleId = (int)TenantAdminRoleCode.SuperAdmin,
            PermissionId = (int)TenantAdminPermissionCode.UpdateTenants
        },
        new TenantAdminRolePermission
        {
            RoleId = (int)TenantAdminRoleCode.SuperAdmin,
            PermissionId = (int)TenantAdminPermissionCode.DeleteTenants
        },
        new TenantAdminRolePermission
        {
            RoleId = (int)TenantAdminRoleCode.Admin,
            PermissionId = (int)TenantAdminPermissionCode.ReadTenants
        },
        new TenantAdminRolePermission
        {
            RoleId = (int)TenantAdminRoleCode.Admin,
            PermissionId = (int)TenantAdminPermissionCode.CreateTenants
        },
        new TenantAdminRolePermission
        {
            RoleId = (int)TenantAdminRoleCode.Admin,
            PermissionId = (int)TenantAdminPermissionCode.UpdateTenants
        },
        new TenantAdminRolePermission
        {
            RoleId = (int)TenantAdminRoleCode.Admin,
            PermissionId = (int)TenantAdminPermissionCode.DeleteTenants
        }
    ];
}
