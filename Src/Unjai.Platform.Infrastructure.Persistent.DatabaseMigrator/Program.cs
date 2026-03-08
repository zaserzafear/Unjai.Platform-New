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

builder.Services.AddPostgresClientExtension(
    builder.Configuration.GetConnectionString(PostgresConfig.DefaultConnectionString),
    builder.Configuration.GetConnectionString(PostgresConfig.ReadConnectionString),
    builder.Configuration.GetConnectionString(PostgresConfig.WriteConnectionString),
    logger);

builder.Services.AddDependencyInjections();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
