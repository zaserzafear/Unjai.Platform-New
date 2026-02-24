using Microsoft.Extensions.DependencyInjection;

namespace Unjai.Platform.Application.Services.TenantAdminRolePermissions;

public static class TenantAdminRolePermissionExtensions
{
    public static void AddTenantAdminRolePermissionService(this IServiceCollection Services)
    {
        Services.AddScoped<TenantAdminRolePermissionService>();
    }
}
