using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Unjai.Platform.Extensions.ObjectMapper;

public static class AutoMapperExtensions
{
    public static IServiceCollection AddCustomAutoMapper(
        this IServiceCollection services,
        params Assembly[] assemblies)
    {
        var profiles = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract &&
                        typeof(AutoMapperProfile).IsAssignableFrom(t))
            .Select(t => (AutoMapperProfile)Activator.CreateInstance(t)!)
            .ToList();

        services.AddSingleton<IObjectMapper>(new ObjectMapper(profiles));

        return services;
    }
}
