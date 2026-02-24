using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Application.Services.TenantAdmins.LoginTenantAdmin;

namespace Unjai.Platform.Application.Services.TenantAdmins.Extensions;

public static class TenantAdminExtensions
{
    public static void AddTenantAdminService(this IServiceCollection services)
    {
        services.AddScoped<LoginTenantAdminV1>();
    }
}
