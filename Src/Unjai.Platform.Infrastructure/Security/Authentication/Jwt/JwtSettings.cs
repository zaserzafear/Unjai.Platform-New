namespace Unjai.Platform.Infrastructure.Security.Authentication.Jwt;

public sealed class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Secret { get; set; } = string.Empty;
    public double ExpireMinutes { get; set; }
    public int ClockSkew { get; set; }
    public string RoleClaimType { get; } = "role";
    public string DisplayNameClaimType { get; } = "displayName";
    public string AccessTokenCookieName { get; } = "AccessToken";
    public string RefreshTokenCookieName { get; } = "RefreshToken";
}
