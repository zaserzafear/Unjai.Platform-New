using System.Security.Cryptography;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;
using Unjai.Platform.Infrastructure.Security.Authentication.Jwt;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.JwtSigningKeys;

public static class JwtSigningKeyExtensions
{
    public static void AddJwtSigningKeyRepositoryExtensions(this IServiceCollection services)
    {
        services.AddScoped<IJwtKeyStore, JwtKeyStore>();
    }

    public static SecurityKey ToSecurityKey(this JwtSigningKey key)
    {
        using var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(key.PublicKeyPem);

        return new ECDsaSecurityKey(ecdsa)
        {
            KeyId = key.KeyId
        };
    }
}
