using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Application.Repositories.TenantAdmins;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.TenantAdmins;

public static class TenantAdminExtensions
{
    public static void AddTenantAdminRepository(this IServiceCollection services)
    {
        services.AddScoped<ITenantAdminRepository, TenantAdminRepository>();
    }
}
