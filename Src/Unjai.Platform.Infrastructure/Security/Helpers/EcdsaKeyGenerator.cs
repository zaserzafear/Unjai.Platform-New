using System.Security.Cryptography;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;

namespace Unjai.Platform.Infrastructure.Security.Helpers;

public static class EcdsaKeyGenerator
{
    public static JwtSigningKey Create()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        var privatePem = ecdsa.ExportPkcs8PrivateKeyPem();
        var publicPem = ecdsa.ExportSubjectPublicKeyInfoPem();

        return new JwtSigningKey
        {
            KeyId = $"key-{DateTime.UtcNow:yyyyMMddHHmmss}",
            PrivateKeyPem = privatePem,
            PublicKeyPem = publicPem,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }
}
