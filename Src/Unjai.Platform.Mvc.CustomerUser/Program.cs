using Unjai.Platform.Application.Extensions.Authentication;
using Unjai.Platform.Application.Helpers;
using Unjai.Platform.Application.Services.CustomerUsers.Extensions;
using Unjai.Platform.Infrastructure.Database.Extensions;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;
using Unjai.Platform.Infrastructure.Redis.Extensions;

var builder = WebApplication.CreateBuilder(args);

using var loggerFactory = LoggerFactory.Create(config =>
{
    config.AddConsole();
    config.AddConfiguration(builder.Configuration.GetSection("Logging"));
});

var logger = loggerFactory.CreateLogger("Program");

builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddControllersWithViews();

var jwtSetting = builder.Configuration
    .GetSection("Jwt")
    .Get<JwtSetting>()
    ?? throw new InvalidOperationException("Jwt configuration missing");

if (string.IsNullOrWhiteSpace(jwtSetting.Secret))
{
    if (builder.Environment.IsDevelopment())
    {
        jwtSetting.Secret = CryptoHelper.GenerateJwtSecret();
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
    .GetSection("ApiKeys")
    .Get<ApiKeyOption>()
    ?? new ApiKeyOption();

if (string.IsNullOrWhiteSpace(apiKeyOption.HealthCheck))
{
    if (builder.Environment.IsDevelopment())
    {
        apiKeyOption.HealthCheck = CryptoHelper.GenerateApiKey();
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

var primary = builder.Configuration.GetConnectionString("UnjaiDb");

if (string.IsNullOrWhiteSpace(primary))
{
    throw new InvalidOperationException(
        "ConnectionString 'UnjaiDb' must be configured.");
}

var read = builder.Configuration.GetConnectionString("UnjaiDbRead");
if (string.IsNullOrWhiteSpace(read))
{
    logger.LogWarning(
        "ConnectionString 'UnjaiDbRead' is missing. Falling back to 'UnjaiDb'.");

    read = primary;
}

var write = builder.Configuration.GetConnectionString("UnjaiDbWrite");
if (string.IsNullOrWhiteSpace(write))
{
    logger.LogWarning(
        "ConnectionString 'UnjaiDbWrite' is missing. Falling back to 'UnjaiDb'.");

    write = primary;
}

builder.Services.AddPostgresClientExtension(
    primary,
    read,
    write);

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

builder.Services.AddRateLimitingExtension();

builder.Services.AddCustomerUserExtension();

var app = builder.Build();

app.UseOutputCache();
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.ApplyMigrations();
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseRouting();

app.UseAuthExtensions();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
