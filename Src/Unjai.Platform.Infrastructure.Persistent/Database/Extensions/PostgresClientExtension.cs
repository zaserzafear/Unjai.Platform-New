using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Extensions;

public static class PostgresClientExtension
{
    public static void AddPostgresClientExtension(
        this IServiceCollection services,
        string defaultConnectionString,
        string readConnectionString,
        string writeConnectionString)
    {
        services.AddUnitOfWork();

        AddHealthChecks(
            services,
            defaultConnectionString,
            readConnectionString,
            writeConnectionString);

        AddDbContexts(
            services,
            defaultConnectionString,
            writeConnectionString);
    }

    private static void AddHealthChecks(
        IServiceCollection services,
        string defaultConnectionString,
        string readConnectionString,
        string writeConnectionString)
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
    }

    private static void AddDbContexts(
        IServiceCollection services,
        string defaultConnectionString,
        string writeConnectionString)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(writeConnectionString));

        services.AddDbContext<ReadDbContext>(options =>
            options
                .UseNpgsql(
                    defaultConnectionString,
                    npgsql =>
                        npgsql.UseQuerySplittingBehavior(
                            QuerySplittingBehavior.SplitQuery))
                .UseQueryTrackingBehavior(
                    QueryTrackingBehavior.NoTracking));

        services.AddDbContext<WriteDbContext>(options =>
            options.UseNpgsql(writeConnectionString));
    }
}
