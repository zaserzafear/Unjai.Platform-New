using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var unjaiDbName = "UnjaiDb";

var postgres = builder.AddPostgres("postgres").WithImageTag("18")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(pgAdmin => pgAdmin.WithLifetime(ContainerLifetime.Persistent))
    .WithEnvironment("POSTGRES_DB", unjaiDbName)
    .WithBindMount(@"..\.container\postgres\initdb", "/docker-entrypoint-initdb.d")
    ;

var creationScript = $$"""
DO
$$
BEGIN
   IF NOT EXISTS (SELECT FROM pg_database WHERE datname = '{{unjaiDbName}}') THEN
      EXECUTE 'CREATE DATABASE {{unjaiDbName}}';
   END IF;
END
$$;
""";

var postgresdb = postgres.AddDatabase(unjaiDbName)
    .WithCreationScript(creationScript);

var redis = builder.AddRedis("Redis")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight(redisInsight => redisInsight.WithLifetime(ContainerLifetime.Persistent));

var configuration = builder.Configuration;
var apiKeyHealthCheck = configuration.GetValue<string>("ApiKeys:HealthCheck");
var rateLimitingSecret = configuration.GetValue<string>("RateLimiting:Secret");

builder.AddProject<Projects.Unjai_Platform_Infrastructure_Persistent_DatabaseMigrator>("unjai-platform-infrastructure-persistent-databasemigrator")
    .WithReference(postgresdb).WaitFor(postgresdb);

builder.AddProject<Projects.Unjai_Platform_Worker_JwtKeyRotation>("unjai-platform-worker-jwtkeyrotation")
    .WithReference(postgresdb).WaitFor(postgresdb)
    .WithReference(redis).WaitFor(redis);

var apiProject = builder.AddProject<Projects.Unjai_Platform_Api>("unjai-platform-api")
    .WithReference(postgresdb).WaitFor(postgresdb)
    .WithReference(redis).WaitFor(redis)
    .WithEnvironment("ApiKeys__HealthCheck", apiKeyHealthCheck)
    .WithEnvironment("RateLimiting__Secret", rateLimitingSecret);

builder.AddProject<Projects.Unjai_Platform_Mvc_CustomerUser>("unjai-platform-mvc-customeruser")
    .WithReference(redis).WaitFor(redis)
    .WithReference(apiProject)
    .WithEnvironment("ApiKeys__HealthCheck", apiKeyHealthCheck)
    .WithEnvironment("RateLimiting__Secret", rateLimitingSecret);

builder.Build().Run();
