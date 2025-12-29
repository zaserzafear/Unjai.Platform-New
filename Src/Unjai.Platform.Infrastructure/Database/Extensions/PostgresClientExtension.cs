using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Unjai.Platform.Infrastructure.Database.Extensions;

public static class PostgresClientExtension
{
    public static void AddPostgresClientExtension(
        this IServiceCollection services,
        string defaultConnectionString,
        string readConnectionString,
        string writeConnectionString
        )
    {
        services.AddHealthChecks()
             .AddNpgSql(
                 connectionString: defaultConnectionString,
                 name: "postgres-default",
                 failureStatus: HealthStatus.Unhealthy,
                 tags: ["ready", "postgres"])
             .AddNpgSql(
                 connectionString: readConnectionString,
                 name: "postgres-read",
                 failureStatus: HealthStatus.Unhealthy,
                 tags: ["ready", "postgres"])
             .AddNpgSql(
                 connectionString: writeConnectionString,
                 name: "postgres-write",
                 failureStatus: HealthStatus.Unhealthy,
                 tags: ["ready", "postgres"]);

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(defaultConnectionString));

        services.AddDbContext<ReadDbContext>(options =>
            options.UseNpgsql(readConnectionString));

        services.AddDbContext<WriteDbContext>(options =>
            options.UseNpgsql(writeConnectionString));
    }
}
