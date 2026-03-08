using Unjai.Platform.Infrastructure.Persistent.Database.Extensions;
using Unjai.Platform.Infrastructure.Persistent.DatabaseMigrator;
using Unjai.Platform.Infrastructure.Persistent.DatabaseMigrator.Extensions;

var builder = Host.CreateApplicationBuilder(args);

using var loggerFactory = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

var logger = loggerFactory.CreateLogger<Program>();

builder.AddServiceDefaults();

var dbPrimary = builder.Configuration.GetConnectionString("UnjaiDb");

if (string.IsNullOrWhiteSpace(dbPrimary))
{
    throw new InvalidOperationException(
        "ConnectionString 'UnjaiDb' must be configured.");
}

var dbRead = builder.Configuration.GetConnectionString("UnjaiDbRead");
if (string.IsNullOrWhiteSpace(dbRead))
{
    logger.LogWarning(
        "ConnectionString 'UnjaiDbRead' is missing. Falling back to 'UnjaiDb'.");

    dbRead = dbPrimary;
}

var dbWrite = builder.Configuration.GetConnectionString("UnjaiDbWrite");
if (string.IsNullOrWhiteSpace(dbWrite))
{
    logger.LogWarning(
        "ConnectionString 'UnjaiDbWrite' is missing. Falling back to 'UnjaiDb'.");

    dbWrite = dbPrimary;
}

builder.Services.AddPostgresClientExtension(
    dbPrimary,
    dbRead,
    dbWrite);

builder.Services.AddDependencyInjections();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
