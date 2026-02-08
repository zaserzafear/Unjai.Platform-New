using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Application.Repositories.Tenants;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.Tenants;

public static class TenantExtensions
{
    public static void AddTenantRepositoryExtensions(this IServiceCollection services)
    {
        services.AddScoped<ITenantRepository, TenantRepository>();
    }
}
