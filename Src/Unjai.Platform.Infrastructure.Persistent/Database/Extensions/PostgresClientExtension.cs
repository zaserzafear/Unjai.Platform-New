using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Extensions;

public static class PostgresClientExtension
{
    public static void AddPostgresClientExtension(
        this IServiceCollection services,
        string? defaultConnectionString,
        string? readConnectionString,
        string? writeConnectionString,
        ILogger logger)
    {
        var connectionStrings = ValidatePostgresConnectionString(
            defaultConnectionString,
            readConnectionString,
            writeConnectionString,
            logger);

        services.AddUnitOfWork();

        AddHealthChecks(
            services,
            connectionStrings.DefaultConnectionString,
            connectionStrings.ReadConnectionString,
            connectionStrings.WriteConnectionString);

        AddDbContexts(
            services,
            connectionStrings.ReadConnectionString,
            connectionStrings.WriteConnectionString);
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

    private static PostgresConnectionStrings ValidatePostgresConnectionString(
        string? defaultConnectionString,
        string? readConnectionString,
        string? writeConnectionString,
        ILogger logger)
    {
        if (string.IsNullOrWhiteSpace(defaultConnectionString))
        {
            throw new InvalidOperationException(
                $"ConnectionString '{PostgresConfig.DefaultConnectionString}' must be configured.");
        }

        if (string.IsNullOrWhiteSpace(readConnectionString))
        {
            logger.LogWarning(
                "ConnectionString '{ConnectionStringName}' is missing. Falling back to '{FallbackConnectionStringName}'.",
                PostgresConfig.ReadConnectionString,
                PostgresConfig.DefaultConnectionString);

            readConnectionString = defaultConnectionString;
        }

        if (string.IsNullOrWhiteSpace(writeConnectionString))
        {
            logger.LogWarning(
                "ConnectionString '{ConnectionStringName}' is missing. Falling back to '{FallbackConnectionStringName}'.",
                PostgresConfig.WriteConnectionString,
                PostgresConfig.DefaultConnectionString);

            writeConnectionString = defaultConnectionString;
        }

        return new PostgresConnectionStrings(
            defaultConnectionString,
            readConnectionString,
            writeConnectionString);
    }

    private sealed record PostgresConnectionStrings(
        string DefaultConnectionString,
        string ReadConnectionString,
        string WriteConnectionString);
}

public static class PostgresConfig
{
    public const string DefaultConnectionString = "UnjaiDb";
    public const string ReadConnectionString = "UnjaiDbRead";
    public const string WriteConnectionString = "UnjaiDbWrite";
}
