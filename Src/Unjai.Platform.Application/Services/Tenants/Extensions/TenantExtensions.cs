using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Application.Services.Tenants.CreateTenant;
using Unjai.Platform.Application.Services.Tenants.GetTenant;
using Unjai.Platform.Application.Services.Tenants.UpdateTenant;

namespace Unjai.Platform.Application.Services.Tenants.Extensions;

public static class TenantExtensions
{
    public static void AddTenantServiceExtension(this IServiceCollection Services)
    {
        Services.AddScoped<CreateTenantV1>();
        Services.AddScoped<GetTenantAllV1>();
        Services.AddScoped<GetTenantByIdV1>();
        Services.AddScoped<UpdateTenantV1>();
    }
}
