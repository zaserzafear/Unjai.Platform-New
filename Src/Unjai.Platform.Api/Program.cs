using System.Globalization;
using System.Security.Cryptography;
using Asp.Versioning;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using Unjai.Platform.Api.Endpoints.Extensions;
using Unjai.Platform.Api.Extensions;
using Unjai.Platform.Api.RateLimiting;
using Unjai.Platform.Application.Services.JwtKeyStores;
using Unjai.Platform.Infrastructure.Caching.Extensions;
using Unjai.Platform.Infrastructure.Persistent.Database.Extensions;
using Unjai.Platform.Infrastructure.Persistent.Seeding;
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;
using Unjai.Platform.Infrastructure.RateLimiting.Configurations;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;
using Unjai.Platform.Infrastructure.Redis.Extensions;
using Unjai.Platform.Infrastructure.Security;
using Unjai.Platform.Infrastructure.Security.Authentication.ApiKey;
using Unjai.Platform.Infrastructure.Security.Authentication.Jwt;
using Unjai.Platform.Infrastructure.Security.Cryptography;
using Unjai.Platform.Infrastructure.Security.Networking.TrustedIp;

var builder = WebApplication.CreateBuilder(args);

using var loggerFactory = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

var logger = loggerFactory.CreateLogger<Program>();

var trustIpSourceOptions = builder.Configuration
    .GetSection(TrustIpSourceConfig.Section)
    .Get<TrustIpSourceOptions>()
    ?? new TrustIpSourceOptions();
builder.Services.AddTrustedIpSources(trustIpSourceOptions);

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

var apiVersions = builder.Configuration
    .GetSection("ApiVersions")
    .Get<string[]>()
    ?? Array.Empty<string>();

foreach (var version in apiVersions)
{
    var apiVersion = new ApiVersion(int.Parse(version, CultureInfo.InvariantCulture));
    var groupName = "v" + apiVersion.MajorVersion;
    builder.Services.AddOpenApi(groupName);
}

builder.Services.AddEndpoints(typeof(Program).Assembly);

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var jwtSetting = builder.Configuration
    .GetSection(JwtSettingConfig.Section)
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt configuration missing");

var apiKeyOptions = builder.Configuration
    .GetSection(ApiKeyConfig.Section)
    .Get<Unjai.Platform.Infrastructure.Security.Authentication.ApiKey.ApiKeyOptions>()
    ?? new Unjai.Platform.Infrastructure.Security.Authentication.ApiKey.ApiKeyOptions();

if (string.IsNullOrWhiteSpace(apiKeyOptions.HealthCheck))
{
    if (builder.Environment.IsDevelopment())
    {
        apiKeyOptions.HealthCheck = CryptoHelper.GenerateSecret(32);

        if (logger.IsEnabled(LogLevel.Critical))
        {
            logger.LogCritical(
                "SECURITY WARNING (DEV ONLY): ApiKeys:HealthCheck was auto-generated. " +
                "COPY THIS VALUE AND STORE IT SECURELY. Value={ApiKey}",
                apiKeyOptions.HealthCheck);
        }
    }
    else
    {
        throw new InvalidOperationException("ApiKeys:HealthCheck must be configured in production.");
    }
}

builder.Services.AddAuthExtensions(jwtSetting, apiKeyOptions);

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

var redisConnectionString =
    builder.Configuration.GetConnectionString("Redis");

if (string.IsNullOrWhiteSpace(redisConnectionString))
{
    throw new InvalidOperationException(
        "Redis connection string 'Redis' was not found or is empty.");
}
else
{
    builder.Services.AddRedisConnection(redisConnectionString);
}

builder.Services.AddCachingExtension();

var rateLimitingOptions =
    builder.Configuration
        .GetSection(RateLimitingConfig.Section)
        .Get<RateLimitingOptions>()
    ?? throw new InvalidOperationException("RateLimiting configuration missing");

if (string.IsNullOrWhiteSpace(rateLimitingOptions.Secret))
{
    if (builder.Environment.IsDevelopment())
    {
        rateLimitingOptions.Secret = CryptoHelper.GenerateSecret(64);

        if (logger.IsEnabled(LogLevel.Critical))
        {
            logger.LogCritical(
                "SECURITY WARNING (DEV ONLY): RateLimiting:Secret was auto-generated. " +
                "COPY THIS VALUE AND STORE IT SECURELY. Value={Secret}",
                rateLimitingOptions.Secret);
        }
    }
    else
    {
        throw new InvalidOperationException(
            "RateLimiting:Secret must be configured in production.");
    }
}

builder.Services.AddRateLimitingExtension(rateLimitingOptions);
builder.Services.AddScoped<IMinimalRateLimitResultFactory, RateLimitResultFactory>();

builder.Services.AddDependencyInjections();

var app = builder.Build();

app.UseTrustedIpSources();

app.UseAuthExtensions();

app.UseOutputCache();

app.MapGet("/.well-known/jwks.json", async (
    IJwtKeyStoreService keyStore) =>
{
    var keys = keyStore.GetAllPublicKeys();

    var jwks = new
    {
        keys = keys.Select(k =>
        {
            using var ecdsa = ECDsa.Create();
            ecdsa.ImportFromPem(k.PublicKeyPem);
            var p = ecdsa.ExportParameters(false);

            return new
            {
                kty = "EC",
                crv = "P-256",
                x = Base64UrlEncoder.Encode(p.Q.X),
                y = Base64UrlEncoder.Encode(p.Q.Y),
                alg = "ES256",
                kid = k.KeyId,
                use = "sig"
            };
        })
    };

    return Results.Ok(jwks);
});

var apiVersionSetBuilder = app.NewApiVersionSet();

foreach (var version in apiVersions)
{
    var apiVersion = new ApiVersion(int.Parse(version, CultureInfo.InvariantCulture));
    apiVersionSetBuilder = apiVersionSetBuilder.HasApiVersion(apiVersion);
}

var apiVersionSet = apiVersionSetBuilder.ReportApiVersions().Build();

var versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            var group = description.GroupName;
            options.AddDocument(group);
        }
    });

    app.ApplyMigrations();
}

await TenantsAdminSeeder.SeedAsync(app.Services);

await app.RunAsync();
