using Microsoft.Extensions.DependencyInjection;

namespace Unjai.Platform.Application.Services.JwtKeyStores;

public static class JwtKeyStoreExtensions
{
    public static void AddJwtKeyStoreService(this IServiceCollection Services)
    {
        Services.AddScoped<JwtKeyStoreService>();
    }
}
