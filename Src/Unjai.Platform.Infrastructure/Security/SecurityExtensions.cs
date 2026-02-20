using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Unjai.Platform.Application.Services.JwtKeyStores;
using Unjai.Platform.Infrastructure.Security.Authentication.ApiKey;
using Unjai.Platform.Infrastructure.Security.Authentication.Jwt;
using Unjai.Platform.Infrastructure.Security.Authentication.Policies;

namespace Unjai.Platform.Infrastructure.Security;

public static class SecurityExtensions
{
    public static void AddAuthExtensions(this IServiceCollection services, JwtSettings jwtSetting, ApiKeyOptions apiKeyOption)
    {
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                RoleClaimType = jwtSetting.RoleClaimType,

                ValidateIssuer = true,
                ValidIssuer = jwtSetting.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtSetting.Audience,

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(jwtSetting.ClockSkew),

                ValidateIssuerSigningKey = true,
                IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
                {
                    using var scope = services.BuildServiceProvider().CreateScope();
                    var keyStore = scope.ServiceProvider.GetRequiredService<IJwtKeyStoreService>();
                    var publicKeys = keyStore.GetAllPublicKeysAsync().Result;

                    var securityKeys = publicKeys.Select(x => x.ToSecurityKey());

                    return securityKeys;
                }
            };

            options.Events = new JwtBearerEvents
            {
                OnMessageReceived = context =>
                {
                    if (context.Request.Cookies.ContainsKey("AccessToken"))
                    {
                        context.Token = context.Request.Cookies["AccessToken"];
                    }

                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(JwtPolicyConfig.HealthPolicyName,
                policy => policy.Requirements.Add(new HealthChecksApiKeyRequirement(apiKeyOption.HealthCheck)));
        });

        services.AddSingleton<IAuthorizationHandler>(new HealthChecksApiKeyHandler(apiKeyOption.HealthCheck));
    }

    public static WebApplication UseAuthExtensions(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
