using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Unjai.Platform.Application.Abstractions.Security.Authentication;
using Unjai.Platform.Application.Abstractions.Security.Cryptography.Ecdsa;
using Unjai.Platform.Domain.Entities.TenantsAdminPermission;
using Unjai.Platform.Infrastructure.Security.Authentication.ApiKey;
using Unjai.Platform.Infrastructure.Security.Authentication.Jwt;
using Unjai.Platform.Infrastructure.Security.Authentication.Policies.HealthChecks;
using Unjai.Platform.Infrastructure.Security.Authentication.Policies.TenantAdmins;
using Unjai.Platform.Infrastructure.Security.Cryptography.Ecdsa;

namespace Unjai.Platform.Infrastructure.Security;

public static class SecurityExtensions
{
    public static void AddSecurityExtensions(this IServiceCollection services)
    {
        services.AddSingleton<IEcdsaKeyGenerator, EcdsaKeyGenerator>();
        services.AddScoped<EcdsaKeyProvider>();
        services.AddScoped<ITokenProvider, TokenProvider>();
    }

    public static void AddAuthenticationExtension(
        this IServiceCollection services,
        Action<JwtSettings> jwtSetting)
    {
        var settings = new JwtSettings();
        jwtSetting(settings);

        ValidateJwtSettings(settings);

        services.Configure(jwtSetting);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.RequireHttpsMetadata = false;
            options.MetadataAddress = settings.MetadataAddress;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                RoleClaimType = settings.RoleClaimType,
                NameClaimType = settings.NameClaimType,

                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,

                ValidateAudience = true,
                ValidAudience = settings.Audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(settings.ClockSkew),
            };

            options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                options.MetadataAddress,
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever { RequireHttps = false })
            {
                RefreshInterval = settings.MetadataRefreshInterval,
                AutomaticRefreshInterval = settings.MetadataAutoRefreshInterval,
            };

            options.Events = new JwtBearerEvents
            {
                OnAuthenticationFailed = context =>
                {
                    var logger = context.HttpContext
                        .RequestServices
                        .GetRequiredService<ILoggerFactory>()
                        .CreateLogger(typeof(SecurityExtensions));

                    var ip = context.HttpContext.Connection.RemoteIpAddress?.ToString();
                    var path = context.HttpContext.Request.Path;

                    var ex = context.Exception;

                    string reason = ex.GetType().Name;

                    logger.LogWarning(ex,
                        "JWT authentication failed. Reason: {Reason}, IP: {IP}, Path: {Path}",
                        reason, ip, path);

                    return Task.CompletedTask;
                }
            };
        });
    }

    private static void ValidateJwtSettings(JwtSettings settings)
    {
        if (string.IsNullOrWhiteSpace(settings.Issuer))
            throw new InvalidOperationException("Jwt:Issuer is required.");

        if (string.IsNullOrWhiteSpace(settings.Audience))
            throw new InvalidOperationException("Jwt:Audience is required.");

        if (string.IsNullOrWhiteSpace(settings.MetadataAddress))
            throw new InvalidOperationException("Jwt:MetadataAddress is required.");

        if (settings.MetadataRefreshInterval <= TimeSpan.Zero)
            throw new InvalidOperationException("Jwt:MetadataRefreshInterval must be greater than 0.");

        if (settings.MetadataAutoRefreshInterval <= TimeSpan.Zero)
            throw new InvalidOperationException("Jwt:MetadataAutoRefreshInterval must be greater than 0.");

        if (settings.AccessTokenExpireMinutes < 0)
            throw new InvalidOperationException("Jwt:AccessTokenExpireMinutes must be greater than or equal to 0.");

        if (settings.RefreshTokenExpireDays < 0)
            throw new InvalidOperationException("Jwt:RefreshTokenExpireDays must be greater than or equal to 0.");

        if (settings.ClockSkew < 0)
            throw new InvalidOperationException("Jwt:ClockSkew must be greater than or equal to 0.");
    }

    public static IServiceCollection AddCoreAuthorizationExtension(
        this IServiceCollection services,
        Action<ApiKeyOptions> apiKeyOption)
    {
        var options = new ApiKeyOptions();
        apiKeyOption(options);

        ValidateApiKeyOptions(options);

        services.Configure(apiKeyOption);

        services.AddAuthorization(optionsBuilder =>
        {
            optionsBuilder.AddPolicy(JwtPolicyConfig.HealthPolicyName,
                policy => policy.Requirements.Add(
                    new HealthChecksApiKeyRequirement(options.HealthCheck)));
        });

        services.AddSingleton<IAuthorizationHandler>(
            new HealthChecksApiKeyHandler(options.HealthCheck));

        return services;
    }

    private static void ValidateApiKeyOptions(ApiKeyOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.HealthCheck))
            throw new InvalidOperationException("ApiKeys:HealthCheck is required.");
    }

    public static IServiceCollection AddTenantAdminAuthorizationExtension(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(TenantAdminPermissionCode.ReadTenants.ToString().ToUpperInvariant(), policy =>
                policy.Requirements.Add(
                    new TenantAdminPermissionRequirement(
                        (int)TenantAdminPermissionCode.ReadTenants)));

            options.AddPolicy(TenantAdminPermissionCode.CreateTenants.ToString().ToUpperInvariant(), policy =>
                policy.Requirements.Add(
                    new TenantAdminPermissionRequirement(
                        (int)TenantAdminPermissionCode.CreateTenants)));

            options.AddPolicy(TenantAdminPermissionCode.UpdateTenants.ToString().ToUpperInvariant(), policy =>
                policy.Requirements.Add(
                    new TenantAdminPermissionRequirement(
                        (int)TenantAdminPermissionCode.UpdateTenants)));

            options.AddPolicy(TenantAdminPermissionCode.DeleteTenants.ToString().ToUpperInvariant(), policy =>
                policy.Requirements.Add(
                    new TenantAdminPermissionRequirement(
                        (int)TenantAdminPermissionCode.DeleteTenants)));
        });

        services.AddScoped<IAuthorizationHandler, TenantAdminPermissionHandler>();

        return services;
    }

    public static WebApplication UseAuthExtension(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
