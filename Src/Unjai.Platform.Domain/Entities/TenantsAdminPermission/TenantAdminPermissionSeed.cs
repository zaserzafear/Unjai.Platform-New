namespace Unjai.Platform.Domain.Entities.TenantsAdminPermission;

public static class TenantAdminPermissionSeed
{
    public static IEnumerable<TenantAdminPermission> GetSeedData()
    {
        yield return new TenantAdminPermission
        {
            Id = (int)TenantAdminPermissionCode.ReadTenants,
            Code = TenantAdminPermissionCode.ReadTenants.ToString().ToUpperInvariant(),
            Name = "Read Tenants"
        };

        yield return new TenantAdminPermission
        {
            Id = (int)TenantAdminPermissionCode.CreateTenants,
            Code = TenantAdminPermissionCode.CreateTenants.ToString().ToUpperInvariant(),
            Name = "Create Tenants"
        };

        yield return new TenantAdminPermission
        {
            Id = (int)TenantAdminPermissionCode.UpdateTenants,
            Code = TenantAdminPermissionCode.UpdateTenants.ToString().ToUpperInvariant(),
            Name = "Update Tenants"
        };

        yield return new TenantAdminPermission
        {
            Id = (int)TenantAdminPermissionCode.DeleteTenants,
            Code = TenantAdminPermissionCode.DeleteTenants.ToString().ToUpperInvariant(),
            Name = "Delete Tenants"
        };
    }
}
