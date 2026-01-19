using Microsoft.Extensions.DependencyInjection;
using Unjai.Platform.Infrastructure.Messaging.Redis;

namespace Unjai.Platform.Infrastructure.Messaging.Extensions;

public static class MessagingExtension
{
    public static void AddRedisMessagingExtension(this IServiceCollection services)
    {
        services.AddSingleton<IDistributedNotificationPublisher, RedisDistributedNotificationPublisher>();
    }
}
