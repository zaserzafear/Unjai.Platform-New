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
        var _jwtSetting = new JwtSettings();
        jwtSetting(_jwtSetting);
        services.Configure(jwtSetting);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.MapInboundClaims = false;
            options.RequireHttpsMetadata = false;
            options.MetadataAddress = _jwtSetting.MetadataAddress;

            options.TokenValidationParameters = new TokenValidationParameters
            {
                RoleClaimType = _jwtSetting.RoleClaimType,
                NameClaimType = _jwtSetting.NameClaimType,

                ValidateIssuer = true,
                ValidIssuer = _jwtSetting.Issuer,

                ValidateAudience = true,
                ValidAudience = _jwtSetting.Audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(_jwtSetting.ClockSkew),
            };

            options.ConfigurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                options.MetadataAddress,
                new OpenIdConnectConfigurationRetriever(),
                new HttpDocumentRetriever { RequireHttps = false })
            {
                RefreshInterval = _jwtSetting.MetadataRefreshInterval,
                AutomaticRefreshInterval = _jwtSetting.MetadataAutoRefreshInterval,
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

    public static IServiceCollection AddCoreAuthorizationExtension(
        this IServiceCollection services,
        Action<ApiKeyOptions> apiKeyOption)
    {
        var _apiKeyOption = new ApiKeyOptions();
        apiKeyOption(_apiKeyOption);
        services.Configure(apiKeyOption);

        services.AddAuthorization(options =>
        {
            options.AddPolicy(JwtPolicyConfig.HealthPolicyName,
                policy => policy.Requirements.Add(
                    new HealthChecksApiKeyRequirement(
                        _apiKeyOption.HealthCheck)));
        });

        services.AddSingleton<IAuthorizationHandler>(new HealthChecksApiKeyHandler(_apiKeyOption.HealthCheck));

        return services;
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
