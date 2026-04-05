using Microsoft.Extensions.Configuration;

var builder = DistributedApplication.CreateBuilder(args);

var unjaiDbName = "UnjaiDb";

var postgres = builder.AddPostgres("postgres").WithImageTag("18.3-alpine")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithPgAdmin(pgAdmin => pgAdmin.WithLifetime(ContainerLifetime.Persistent))
    .WithEnvironment("POSTGRES_DB", unjaiDbName)
    .WithBindMount(@"..\.container\postgres\initdb", "/docker-entrypoint-initdb.d");

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

var redis = builder.AddRedis("Redis").WithImageTag("8.6-alpine")
    .WithLifetime(ContainerLifetime.Persistent)
    .WithRedisInsight(redisInsight => redisInsight.WithLifetime(ContainerLifetime.Persistent));

var rabbitmq = builder.AddRabbitMQ("RabbitMQ").WithImageTag("4.2-alpine")
    .WithLifetime(ContainerLifetime.Persistent);

var configuration = builder.Configuration;
var apiKeyHealthCheck = configuration.GetValue<string>("ApiKeys:HealthCheck");
var rateLimitingSecret = configuration.GetValue<string>("RateLimiting:Secret");

var migrator = builder
    .AddProject<Projects.Unjai_Platform_Infrastructure_Persistent_DatabaseMigrator>(
        "unjai-db-migrator")
    .WithReference(postgresdb)
    .WaitFor(postgresdb);

var jwtKeyRotation = builder
    .AddProject<Projects.Unjai_Platform_Worker_JwtKeyRotation>(
        "unjai-jwt-rotation")
    .WaitForCompletion(migrator)
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithReference(redis)
    .WaitFor(redis);

var apiProject = builder
    .AddProject<Projects.Unjai_Platform_Api>("unjai-api")
    .WaitForCompletion(jwtKeyRotation)
    .WithReference(postgresdb)
    .WaitFor(postgresdb)
    .WithReference(redis)
    .WaitFor(redis)
    .WithEnvironment("ApiKeys__HealthCheck", apiKeyHealthCheck)
    .WithEnvironment("RateLimiting__Secret", rateLimitingSecret);

builder
    .AddProject<Projects.Unjai_Platform_Mvc_CustomerUser>("unjai-mvc-customer")
    .WithReference(apiProject)
    .WithReference(redis)
    .WaitFor(redis)
    .WaitFor(apiProject)
    .WithEnvironment("ApiKeys__HealthCheck", apiKeyHealthCheck)
    .WithEnvironment("RateLimiting__Secret", rateLimitingSecret);

builder.Build().Run();
