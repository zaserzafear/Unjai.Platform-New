using Unjai.Platform.Infrastructure.Persistent.Repositories.TenantAdmins;

namespace Unjai.Platform.Infrastructure.Persistent.DatabaseMigrator.Extensions;

internal static class DependencyInjections
{
    public static IServiceCollection AddDependencyInjections(this IServiceCollection services)
    {
        services.AddTenantAdminRepository();

        return services;
    }
}
