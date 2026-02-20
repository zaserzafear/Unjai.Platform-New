using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Unjai.Platform.Application.Repositories.JwtKeyStores;
using Unjai.Platform.Infrastructure.Security.Cryptography.Ecdsa;

namespace Unjai.Platform.Infrastructure.Security.Authentication.Jwt;

public sealed class JwtTokenIssuer(IJwtKeyStoreRepository keyStore)
{
    public string IssueToken(IEnumerable<Claim> claims)
    {
        var key = keyStore.GetActiveKey();

        var credentials = new SigningCredentials(
            key.ToPrivateKey(),
            SecurityAlgorithms.EcdsaSha256
        );

        var token = new JwtSecurityToken(
            issuer: "unjai-api",
            audience: "unjai-mvc",
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials
        );

        token.Header["kid"] = key.KeyId;

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
