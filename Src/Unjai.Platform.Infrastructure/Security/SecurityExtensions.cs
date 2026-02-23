using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Unjai.Platform.Application.Abstractions.Security.Authentication;
using Unjai.Platform.Application.Abstractions.Security.Cryptography.Ecdsa;
using Unjai.Platform.Application.Services.JwtKeyStores;
using Unjai.Platform.Infrastructure.Security.Authentication.ApiKey;
using Unjai.Platform.Infrastructure.Security.Authentication.Jwt;
using Unjai.Platform.Infrastructure.Security.Authentication.Policies;
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

    public static void AddAuthExtensions(this IServiceCollection services, Action<JwtSettings> jwtSetting, Action<ApiKeyOptions> apiKeyOption)
    {
        var _jwtSetting = new JwtSettings();
        jwtSetting(_jwtSetting);
        services.Configure(jwtSetting);

        var _apiKeyOption = new ApiKeyOptions();
        apiKeyOption(_apiKeyOption);
        services.Configure(apiKeyOption);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
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

                ValidateIssuerSigningKey = true,
                IssuerSigningKeyResolver = (token, securityToken, kid, parameters) =>
                    parameters.IssuerSigningKeys?.Where(k => k.KeyId == kid)
            };
        });

        services.PostConfigure<JwtBearerOptions>(
            JwtBearerDefaults.AuthenticationScheme,
            async options =>
            {
                var jwtKeyStoreService = services
                .BuildServiceProvider()
                .GetRequiredService<JwtKeyStoreService>();

                var ecdsaKeyProvider = services
                .BuildServiceProvider()
                .GetRequiredService<EcdsaKeyProvider>();

                options.TokenValidationParameters.IssuerSigningKeys = jwtKeyStoreService
                .GetAllPublicKeys()
                .Select(k => ecdsaKeyProvider.ToPublicKey(k));
            });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(JwtPolicyConfig.HealthPolicyName,
                policy => policy.Requirements.Add(new HealthChecksApiKeyRequirement(_apiKeyOption.HealthCheck)));
        });

        services.AddSingleton<IAuthorizationHandler>(new HealthChecksApiKeyHandler(_apiKeyOption.HealthCheck));
    }

    public static WebApplication UseAuthExtensions(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}
