using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;

namespace Unjai.Platform.Infrastructure.Security.Authentication.Jwt;

internal static class JwtKeyStoreExtensions
{
    internal static SecurityKey ToSecurityKey(this JwtSigningKey key)
    {
        using var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(key.PublicKeyPem);

        return new ECDsaSecurityKey(ecdsa)
        {
            KeyId = key.KeyId
        };
    }
}
