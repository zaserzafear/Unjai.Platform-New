using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Unjai.Platform.Infrastructure.Redis.Extensions;

public static class RedisConnectionExtension
{
    public static void AddRedisConnection(
        this IServiceCollection services,
        string redisConnectionString)
    {
        services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
    }
}
