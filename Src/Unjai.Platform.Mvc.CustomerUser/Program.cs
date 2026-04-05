using System.Diagnostics;
using Microsoft.Extensions.Options;
using Unjai.Platform.Infrastructure.RateLimiting.Abstractions;
using Unjai.Platform.Infrastructure.RateLimiting.AspNetCore.Delegates;
using Unjai.Platform.Infrastructure.RateLimiting.Configurations;
using Unjai.Platform.Infrastructure.RateLimiting.Extensions;
using Unjai.Platform.Infrastructure.Redis.Extensions;
using Unjai.Platform.Infrastructure.Security;
using Unjai.Platform.Infrastructure.Security.Authentication.ApiKey;
using Unjai.Platform.Infrastructure.Security.Authentication.Jwt;
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

builder.Services.AddSingleton(_ =>
    new ActivitySource(builder.Environment.ApplicationName));

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
    });

builder.Services.AddRedisConnection(builder.Configuration.GetConnectionString(RedisConfig.ConnectionString));

builder.Services.AddRateLimitingExtension(
    rateLimit =>
    {
        builder.Configuration
            .GetSection(RateLimitingConfig.Section)
            .Bind(rateLimit);
    });

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

app.UseRouting();

app.UseAuthExtension();

app.UseOutputCache();

app.MapDefaultEndpoints();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
}
else
{
    app.UseExceptionHandler("/Home/Error");
}

await app.RunAsync();
