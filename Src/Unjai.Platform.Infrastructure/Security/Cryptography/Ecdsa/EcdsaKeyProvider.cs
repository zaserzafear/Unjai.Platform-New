using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;

namespace Unjai.Platform.Infrastructure.Security.Cryptography.Ecdsa;

public sealed class EcdsaKeyProvider
{
    private readonly ECDsa ecdsa;

    public EcdsaKeyProvider()
    {
        ecdsa = ECDsa.Create();
    }

    public SecurityKey ToPrivateKey(JwtSigningKey key)
    {
        ecdsa.ImportFromPem(key.PrivateKeyPem);
        return new ECDsaSecurityKey(ecdsa)
        {
            KeyId = key.KeyId
        };
    }

    public ECDsaSecurityKey ToPublicKey(JwtSigningKey key)
    {
        ecdsa.ImportFromPem(key.PublicKeyPem);
        return new ECDsaSecurityKey(ecdsa)
        {
            KeyId = key.KeyId
        };
    }

    public void Dispose()
    {
        ecdsa?.Dispose();
    }
}
