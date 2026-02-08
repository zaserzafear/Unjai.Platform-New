using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Domain.Abstractions;
using Unjai.Platform.Infrastructure.Persistent.Repositories;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Extensions;

internal static class DependencyInjections
{
    public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }
}
