using Unjai.Platform.Application.Services.CustomerUsers.Extensions;
using Unjai.Platform.Application.Services.JwtKeyStores;
using Unjai.Platform.Application.Services.TenantAdminRolePermissions;
using Unjai.Platform.Application.Services.TenantAdmins.Extensions;
using Unjai.Platform.Application.Services.Tenants.Extensions;
using Unjai.Platform.Infrastructure.Persistent.Repositories.JwtSigningKeys;
using Unjai.Platform.Infrastructure.Persistent.Repositories.TenantAdminRolePermissions;
using Unjai.Platform.Infrastructure.Persistent.Repositories.TenantAdmins;
using Unjai.Platform.Infrastructure.Persistent.Repositories.Tenants;
using Unjai.Platform.Infrastructure.Security;

namespace Unjai.Platform.Api.Extensions;

internal static class DependencyInjections
{
    public static IServiceCollection AddDependencyInjections(this IServiceCollection services)
    {
        services.AddJwtSigningKeyRepository();
        services.AddJwtKeyStoreService();
        services.AddSecurityExtensions();

        services.AddCustomerUserServiceExtension();

        services.AddTenantRepositoryExtensions();
        services.AddTenantServiceExtension();

        services.AddTenantAdminRepository();
        services.AddTenantAdminService();

        services.AddTenantAdminRolePermissionRepository();
        services.AddTenantAdminRolePermissionService();

        return services;
    }
}
