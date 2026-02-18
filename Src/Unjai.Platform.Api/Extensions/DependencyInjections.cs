using Unjai.Platform.Application.Services.CustomerUsers.Extensions;
using Unjai.Platform.Application.Services.Tenants.Extensions;
using Unjai.Platform.Infrastructure.Persistent.Repositories.JwtSigningKeys;
using Unjai.Platform.Infrastructure.Persistent.Repositories.Tenants;

namespace Unjai.Platform.Api.Extensions;

public static class DependencyInjections
{
    public static IServiceCollection AddDependencyInjections(this IServiceCollection services)
    {
        services.AddCustomerUserServiceExtension();
        services.AddTenantServiceExtension();
        services.AddTenantRepositoryExtensions();
        services.AddJwtSigningKeyRepositoryExtensions();

        return services;
    }
}
