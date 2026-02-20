using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Application.Services.JwtKeyStores;

namespace Unjai.Platform.Application.Repositories.JwtKeyStores;

public static class JwtKeyStoreExtension
{
    public static void AddJwtKeyStoreExtension(this IServiceCollection Services)
    {
        Services.AddScoped<IJwtKeyStoreService, JwtKeyStoreService>();
    }
}
