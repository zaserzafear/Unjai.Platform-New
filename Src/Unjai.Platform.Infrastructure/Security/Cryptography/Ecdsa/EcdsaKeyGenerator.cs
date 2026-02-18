using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;

namespace Unjai.Platform.Infrastructure.Security.Cryptography.Ecdsa;

public static class EcdsaKeyGenerator
{
    public static JwtSigningKey Create()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        var privatePem = ecdsa.ExportPkcs8PrivateKeyPem();
        var publicPem = ecdsa.ExportSubjectPublicKeyInfoPem();
        var kid = GenerateKid(ecdsa);

        return new JwtSigningKey
        {
            KeyId = kid,
            PrivateKeyPem = privatePem,
            PublicKeyPem = publicPem,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    private static string GenerateKid(ECDsa ecdsa)
    {
        var p = ecdsa.ExportParameters(false);

        var jwk = $$"""
        {"crv":"P-256","kty":"EC","x":"{{Base64UrlEncoder.Encode(p.Q.X)}}","y":"{{Base64UrlEncoder.Encode(p.Q.Y)}}"}
        """;

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(jwk));
        return Base64UrlEncoder.Encode(hash);
    }
}
