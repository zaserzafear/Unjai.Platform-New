using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Application.Services.Tenants.CreateTenant;
using Unjai.Platform.Application.Services.Tenants.GetTenant;
using Unjai.Platform.Application.Services.Tenants.UpdateTenant;

namespace Unjai.Platform.Application.Services.Tenants.Extensions;

public static class TenantExtension
{
    public static void AddTenantServiceExtension(this IServiceCollection Services)
    {
        Services.AddScoped<ICreateTenantV1, CreateTenantV1>();
        Services.AddScoped<IGetTenantAllV1, GetTenantAllV1>();
        Services.AddScoped<IGetTenantByIdV1, GetTenantByIdV1>();
        Services.AddScoped<IUpdateTenantV1, UpdateTenantV1>();
    }
}
