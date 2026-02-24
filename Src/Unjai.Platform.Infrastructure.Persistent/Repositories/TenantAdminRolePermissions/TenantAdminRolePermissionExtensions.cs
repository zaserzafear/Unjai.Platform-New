using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Application.Repositories.TenantAdminRolePermissions;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.TenantAdminRolePermissions;

public static class TenantAdminRolePermissionExtensions
{
    public static void AddTenantAdminRolePermissionRepository(this IServiceCollection services)
    {
        services.AddScoped<ITenantAdminRolePermissionRepository, TenantAdminRolePermissionRepository>();
    }
}
