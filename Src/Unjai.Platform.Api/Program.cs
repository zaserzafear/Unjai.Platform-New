using System.Diagnostics;
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
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;
using Unjai.Platform.Infrastructure.RateLimiting.Configurations;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;
using Unjai.Platform.Infrastructure.Redis.Extensions;
using Unjai.Platform.Infrastructure.Security;
using Unjai.Platform.Infrastructure.Security.Authentication.ApiKey;
using Unjai.Platform.Infrastructure.Security.Authentication.Jwt;
using Unjai.Platform.Infrastructure.Security.Networking.TrustedIp;

var builder = WebApplication.CreateBuilder(args);

using var loggerFactory = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

var logger = loggerFactory.CreateLogger<Program>();

builder.Services.AddSingleton(_ =>
    new ActivitySource(builder.Environment.ApplicationName));

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

builder.Services.AddAuthenticationExtension(
    jwt =>
    {
        builder.Configuration
            .GetSection(JwtSettingConfig.Section)
            .Bind(jwt);
    });

builder.Services.AddCoreAuthorizationExtension(
    apiKey =>
    {
        builder.Configuration
            .GetSection(ApiKeyConfig.Section)
            .Bind(apiKey);
    })
    .AddTenantAdminAuthorizationExtension();

builder.Services.AddPostgresClientExtension(
    builder.Configuration.GetConnectionString(PostgresConfig.DefaultConnectionString),
    builder.Configuration.GetConnectionString(PostgresConfig.ReadConnectionString),
    builder.Configuration.GetConnectionString(PostgresConfig.WriteConnectionString),
    logger);

builder.Services.AddRedisConnection(builder.Configuration.GetConnectionString(RedisConfig.ConnectionString));

builder.Services.AddCachingExtension();

builder.Services.AddRateLimitingExtension(
    rateLimit =>
    {
        builder.Configuration
            .GetSection(RateLimitingConfig.Section)
            .Bind(rateLimit);
    });

builder.Services.AddScoped<IMinimalRateLimitResultFactory, RateLimitResultFactory>();

builder.Services.AddDependencyInjections();

var app = builder.Build();

app.UseTrustedIpSources();

app.UseAuthExtension();

app.UseOutputCache();

app.MapGet("/.well-known/openid-configuration", (HttpContext ctx) =>
{
    var issuer = $"{ctx.Request.Scheme}://{ctx.Request.Host}";

    return Results.Ok(new
    {
        issuer,
        jwks_uri = $"{issuer}/.well-known/jwks.json",
        id_token_signing_alg_values_supported = OpenIdMetadata.SigningAlgorithms
    });
});

app.MapGet("/.well-known/jwks.json", async (JwtKeyStoreService keyStore) =>
{
    var keys = await keyStore.GetAllNotExpiredKeysAsync();

    var jwksList = new List<object>();

    foreach (var k in keys)
    {
        using var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(k.PublicKeyPem);

        var securityKey = new ECDsaSecurityKey(ecdsa)
        {
            KeyId = k.KeyId
        };

        var jwk = JsonWebKeyConverter.ConvertFromECDsaSecurityKey(securityKey);

        jwksList.Add(new
        {
            kid = jwk.Kid,
            kty = jwk.Kty,
            alg = SecurityAlgorithms.EcdsaSha256,
            use = "sig",
            crv = jwk.Crv,
            x = jwk.X,
            y = jwk.Y
        });
    }

    // Return object ตามโครงสร้างมาตรฐานของ JWKS
    return Results.Ok(new { keys = jwksList });
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
}

await app.RunAsync();

static class OpenIdMetadata
{
    public static readonly string[] SigningAlgorithms =
    {
        SecurityAlgorithms.EcdsaSha256
    };
}
