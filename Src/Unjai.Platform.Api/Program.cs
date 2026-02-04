using System.Globalization;
using Asp.Versioning;
using Scalar.AspNetCore;
using Unjai.Platform.Api.Endpoints.Extensions;
using Unjai.Platform.Api.RateLimiting;
using Unjai.Platform.Application.Helpers;
using Unjai.Platform.Application.Services.CustomerUsers.Extensions;
using Unjai.Platform.Infrastructure.Caching.Extensions;
using Unjai.Platform.Infrastructure.Database.Extensions;
using Unjai.Platform.Infrastructure.Messaging.Extensions;
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;
using Unjai.Platform.Infrastructure.RateLimiting.Configurations;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;
using Unjai.Platform.Infrastructure.Redis.Extensions;
using Unjai.Platform.Infrastructure.Security.Auth.Configurations;
using Unjai.Platform.Infrastructure.Security.Auth.Extensions;
using Unjai.Platform.Infrastructure.Security.Forwarding.Extensions;
using Unjai.Platform.Infrastructure.Security.TrustedIpSources.Configurations;

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
    .Get<JwtSetting>()
    ?? throw new InvalidOperationException("Jwt configuration missing");

if (string.IsNullOrWhiteSpace(jwtSetting.Secret))
{
    if (builder.Environment.IsDevelopment())
    {
        jwtSetting.Secret = CryptoHelper.GenerateSecret(64);
        logger.LogCritical(
            "SECURITY WARNING (DEV ONLY): Jwt:Secret was auto-generated. " +
            "COPY THIS VALUE AND STORE IT SECURELY. Value={Secret}",
            jwtSetting.Secret);
    }
    else
    {
        throw new InvalidOperationException("Jwt:Secret must be configured in production.");
    }
}

var apiKeyOption = builder.Configuration
    .GetSection(ApiKeyConfig.Section)
    .Get<ApiKeyOption>()
    ?? new ApiKeyOption();

if (string.IsNullOrWhiteSpace(apiKeyOption.HealthCheck))
{
    if (builder.Environment.IsDevelopment())
    {
        apiKeyOption.HealthCheck = CryptoHelper.GenerateSecret(32);
        logger.LogCritical(
            "SECURITY WARNING (DEV ONLY): ApiKeys:HealthCheck was auto-generated. " +
            "COPY THIS VALUE AND STORE IT SECURELY. Value={ApiKey}",
            apiKeyOption.HealthCheck);
    }
    else
    {
        throw new InvalidOperationException("ApiKeys:HealthCheck must be configured in production.");
    }
}

builder.Services.AddAuthExtensions(jwtSetting, apiKeyOption);

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
builder.Services.AddRedisMessagingExtension();

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

        logger.LogCritical(
            "SECURITY WARNING (DEV ONLY): RateLimiting:Secret was auto-generated. " +
            "COPY THIS VALUE AND STORE IT SECURELY. Value={Secret}",
            rateLimitingOptions.Secret);
    }
    else
    {
        throw new InvalidOperationException(
            "RateLimiting:Secret must be configured in production.");
    }
}

builder.Services.AddRateLimitingExtension(rateLimitingOptions);
builder.Services.AddScoped<IMinimalRateLimitResultFactory, RateLimitResultFactory>();

builder.Services.AddCustomerUserExtension();

var app = builder.Build();

app.UseTrustedIpSources();

app.UseAuthExtensions();

app.UseOutputCache();

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

await app.RunAsync();
