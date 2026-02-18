using Microsoft.Extensions.Options;
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;
using Unjai.Platform.Infrastructure.RateLimiting.AspNetCore.Delegates;
using Unjai.Platform.Infrastructure.RateLimiting.Configurations;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;
using Unjai.Platform.Infrastructure.Redis.Extensions;
using Unjai.Platform.Infrastructure.Security;
using Unjai.Platform.Infrastructure.Security.Authentication.ApiKey;
using Unjai.Platform.Infrastructure.Security.Authentication.Jwt;
using Unjai.Platform.Infrastructure.Security.Cryptography;
using Unjai.Platform.Infrastructure.Security.Networking.TrustedIp;
using Unjai.Platform.Mvc.CustomerUser.Configurations;
using Unjai.Platform.Mvc.CustomerUser.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

using var loggerFactory = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

var logger = loggerFactory.CreateLogger<Program>();

builder.AddServiceDefaults();

builder.Services.AddHttpContextAccessor();

var trustIpSourceOptions = builder.Configuration
    .GetSection(TrustIpSourceConfig.Section)
    .Get<TrustIpSourceOptions>()
    ?? new TrustIpSourceOptions();
builder.Services.AddTrustedIpSources(trustIpSourceOptions);

// Add services to the container.
builder.Services.Configure<RouteOptions>(options =>
{
    options.LowercaseUrls = true;
});
builder.Services.AddControllersWithViews();

var jwtSetting = builder.Configuration
    .GetSection(JwtSettingConfig.Section)
    .Get<JwtSettings>()
    ?? throw new InvalidOperationException("Jwt configuration missing");

var apiKeyOptions = builder.Configuration
    .GetSection(ApiKeyConfig.Section)
    .Get<ApiKeyOptions>()
    ?? new ApiKeyOptions();

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
builder.Services.AddScoped<IMvcRateLimitResultFactory, MvcRateLimitResultFactory>();

builder.Services.AddOptions<ApiOptions>()
    .Configure<IConfiguration>((options, configuration) =>
    {
        var raw =
            configuration["Api:BaseUrl"]
            ?? configuration["UNJAI_PLATFORM_API_HTTP"];

        if (string.IsNullOrWhiteSpace(raw))
        {
            throw new InvalidOperationException(
                "Configuration value 'Api:BaseUrl' is not set. Configure 'Api:BaseUrl' or provide it via the Aspire environment variable.");
        }

        if (!Uri.TryCreate(raw, UriKind.Absolute, out var uri))
        {
            throw new InvalidOperationException(
                $"Configuration value 'Api:BaseUrl' is invalid. The value '{raw}' must be an absolute URI.");
        }

        options.BaseUrl = uri;
    })
    .Validate(
        options => options.BaseUrl.IsAbsoluteUri,
        "Configuration value 'Api:BaseUrl' must be an absolute URI.");

builder.Services.AddHttpClient(HttpClientNames.InternalApi, (sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;

    client.BaseAddress = options.BaseUrl;
}).AddHttpMessageHandler<RateLimitContextHandler>();

var app = builder.Build();

app.UseTrustedIpSources();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();

app.UseAuthExtensions();

app.UseOutputCache();

app.MapDefaultEndpoints();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

await app.RunAsync();
