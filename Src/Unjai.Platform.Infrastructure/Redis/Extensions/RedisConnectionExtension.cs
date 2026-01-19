using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Unjai.Platform.Infrastructure.Redis.Extensions;

public static class RedisConnectionExtension
{
    public static void AddRedisConnection(
        this IServiceCollection services,
        string redisConnectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(
            ConnectionMultiplexer.Connect(redisConnectionString));

        services.AddStackExchangeRedisCache(options =>
        {
            options.ConnectionMultiplexerFactory = () =>
            {
                var provider = services.BuildServiceProvider();
                var multiplexer = provider.GetRequiredService<IConnectionMultiplexer>();

                return Task.FromResult(multiplexer);
            };
        });
    }
}
