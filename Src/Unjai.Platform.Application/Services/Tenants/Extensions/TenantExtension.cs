using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Application.Services.Tenants.CreateTenant;
using Unjai.Platform.Application.Services.Tenants.GetTenant;

namespace Unjai.Platform.Application.Services.Tenants.Extensions;

public static class TenantExtension
{
    public static void AddTenantServiceExtension(this IServiceCollection Services)
    {
        Services.AddScoped<ICreateTenantV1, CreateTenantV1>();
        Services.AddScoped<IGetTenantV1, GetTenantV1>();
    }
}
