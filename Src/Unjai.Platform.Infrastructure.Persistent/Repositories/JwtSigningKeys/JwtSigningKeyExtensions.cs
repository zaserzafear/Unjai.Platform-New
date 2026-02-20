using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Application.Repositories.JwtKeyStores;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.JwtSigningKeys;

public static class JwtSigningKeyExtensions
{
    public static void AddJwtSigningKeyRepositoryExtensions(this IServiceCollection services)
    {
        services.AddScoped<IJwtKeyStoreRepository, JwtKeyStoreRepository>();
    }
}
