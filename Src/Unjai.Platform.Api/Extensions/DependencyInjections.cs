using Unjai.Platform.Application.Repositories.JwtKeyStores;
using Unjai.Platform.Application.Services.CustomerUsers.Extensions;
using Unjai.Platform.Application.Services.Tenants.Extensions;
using Unjai.Platform.Infrastructure.Persistent.Repositories.JwtSigningKeys;
using Unjai.Platform.Infrastructure.Persistent.Repositories.Tenants;
using Unjai.Platform.Infrastructure.Security;

namespace Unjai.Platform.Api.Extensions;

public static class DependencyInjections
{
    public static IServiceCollection AddDependencyInjections(this IServiceCollection services)
    {
        services.AddJwtKeyStoreExtension();
        services.AddJwtSigningKeyRepositoryExtensions();
        services.AddSecurityExtensions();

        services.AddCustomerUserServiceExtension();
        services.AddTenantServiceExtension();
        services.AddTenantRepositoryExtensions();

        return services;
    }
}
