using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Application.Services.CustomerUsers.GetCustomerUser;

namespace Unjai.Platform.Application.Services.CustomerUsers.Extensions;

public static class CustomerUserExtension
{
    public static void AddCustomerUserExtension(this IServiceCollection Services)
    {
        Services.AddScoped<IGetCustomerUserV1, GetCustomerUserV1>();
    }
}
