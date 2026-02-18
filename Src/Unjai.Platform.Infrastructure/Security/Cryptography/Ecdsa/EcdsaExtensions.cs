using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;

namespace Unjai.Platform.Infrastructure.Security.Cryptography.Ecdsa;

public static class EcdsaExtensions
{
    public static SecurityKey ToPrivateKey(this JwtSigningKey key)
    {
        using var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(key.PrivateKeyPem);

        return new ECDsaSecurityKey(ecdsa)
        {
            KeyId = key.KeyId
        };
    }

    public static SecurityKey ToPublicKey(this JwtSigningKey key)
    {
        using var ecdsa = ECDsa.Create();
        ecdsa.ImportFromPem(key.PublicKeyPem);

        return new ECDsaSecurityKey(ecdsa)
        {
            KeyId = key.KeyId
        };
    }
}
