using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Unjai.Platform.Application.Abstractions.Cryptography.Ecdsa;

namespace Unjai.Platform.Infrastructure.Security.Cryptography.Ecdsa;

internal sealed class EcdsaKeyGenerator : IEcdsaKeyGenerator
{
    public (string privatePem, string publicPem, string kid) Create()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        var privatePem = ecdsa.ExportPkcs8PrivateKeyPem();
        var publicPem = ecdsa.ExportSubjectPublicKeyInfoPem();
        var kid = GenerateKid(ecdsa);

        return (privatePem, publicPem, kid);
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
