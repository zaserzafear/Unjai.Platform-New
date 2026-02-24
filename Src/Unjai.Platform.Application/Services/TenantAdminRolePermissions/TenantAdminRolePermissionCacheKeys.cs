namespace Unjai.Platform.Application.Services.TenantAdminRolePermissions;

internal static class TenantAdminRolePermissionCacheKeys
{
    public static string GetByRoleId(int id) => $"TenantAdminRolePermission:GetByRoleId:{id}";
}
