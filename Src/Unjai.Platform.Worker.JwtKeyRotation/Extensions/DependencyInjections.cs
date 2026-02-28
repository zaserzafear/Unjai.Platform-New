using Unjai.Platform.Application.Services.JwtKeyStores;
using Unjai.Platform.Infrastructure.Persistent.Repositories.JwtSigningKeys;
using Unjai.Platform.Infrastructure.Security;

namespace Unjai.Platform.Worker.JwtKeyRotation.Extensions;

internal static class DependencyInjections
{
    public static IServiceCollection AddDependencyInjections(this IServiceCollection services)
    {
        services.AddJwtSigningKeyRepository();
        services.AddJwtKeyStoreService();
        services.AddSecurityExtensions();

        return services;
    }
}
