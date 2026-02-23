using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Unjai.Platform.Application.Abstractions.Security.Authentication;
using Unjai.Platform.Application.Services.JwtKeyStores;
using Unjai.Platform.Domain.Entities.TenantsAdmin;
using Unjai.Platform.Infrastructure.Security.Cryptography.Ecdsa;

namespace Unjai.Platform.Infrastructure.Security.Authentication.Jwt;

internal sealed class TokenProvider(
    IOptions<JwtSettings> jwtSettings,
    JwtKeyStoreService keyStore,
    EcdsaKeyProvider ecdsaKeyProvider)
    : ITokenProvider
{
    public async Task<(string token, long expires)> IssueAccessToken(TenantAdmin entity, CancellationToken ct)
    {
        var jwtSettingsValue = jwtSettings.Value;

        var claims = new List<Claim>
            {
                new Claim(jwtSettingsValue.SubClaimType, entity.Id.ToString()),
                new Claim(jwtSettingsValue.NameClaimType, entity.Username),
                new Claim(jwtSettingsValue.RoleClaimType, entity.Role.Code)
            };

        return await IssuerToken(claims, jwtSettingsValue, ct);
    }

    private async Task<(string token, long expires)> IssuerToken(IEnumerable<Claim> claims, JwtSettings settings, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(settings.AccessTokenExpireMinutes);
        var expiresUnix = new DateTimeOffset(expires).ToUnixTimeSeconds();

        var key = await keyStore.GetActiveNotExpiredKey(ct)
            ?? throw new InvalidOperationException(
                "No active, non-expired JWT signing key found.");

        var credentials = new SigningCredentials(
            ecdsaKeyProvider.ToPrivateKey(key),
            SecurityAlgorithms.EcdsaSha256
        );

        var token = new JwtSecurityToken(
            issuer: settings.Issuer,
            audience: settings.Audience,
            claims: claims,
            expires: expires,
            notBefore: now,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return (tokenString, expiresUnix);
    }
}
