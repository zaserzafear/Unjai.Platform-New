using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Extensions;

public static class MigrationExtensions
{
    public static void ApplyMigrations(this IServiceProvider services)
    {
        ArgumentNullException.ThrowIfNull(services);

        using var scope = services.CreateScope();

        ApplyMigration<WriteDbContext>(scope);
    }

    private static void ApplyMigration<TDbContext>(IServiceScope scope)
        where TDbContext : DbContext
    {
        var context = scope.ServiceProvider.GetRequiredService<TDbContext>();
        context.Database.Migrate();
    }
}
