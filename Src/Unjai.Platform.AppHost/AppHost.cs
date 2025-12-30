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
var jwtSecret = configuration.GetValue<string>("Jwt:Secret");
var apiKeyHealthCheck = configuration.GetValue<string>("ApiKeys:HealthCheck");

builder.AddProject<Projects.Unjai_Platform_Api>("unjai-platform-api")
    .WithReference(postgresdb).WaitFor(postgresdb)
    .WithReference(redis).WaitFor(redis)
    .WithEnvironment("Jwt__Secret", jwtSecret)
    .WithEnvironment("ApiKeys__HealthCheck", apiKeyHealthCheck);

builder.AddProject<Projects.Unjai_Platform_Mvc_CustomerUser>("unjai-platform-mvc-customeruser")
    .WithReference(postgresdb).WaitFor(postgresdb)
    .WithReference(redis).WaitFor(redis)
    .WithEnvironment("Jwt__Secret", jwtSecret)
    .WithEnvironment("ApiKeys__HealthCheck", apiKeyHealthCheck);

builder.Build().Run();
