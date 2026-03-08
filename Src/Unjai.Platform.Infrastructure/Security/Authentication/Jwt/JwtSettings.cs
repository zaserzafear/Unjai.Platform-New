namespace Unjai.Platform.Infrastructure.Security.Authentication.Jwt;

public sealed class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string MetadataAddress { get; set; } = string.Empty;
    public TimeSpan MetadataRefreshInterval { get; set; }
    public TimeSpan MetadataAutoRefreshInterval { get; set; }
    public double AccessTokenExpireMinutes { get; set; }
    public double RefreshTokenExpireDays { get; set; }
    public int ClockSkew { get; set; }

    public string SubClaimType { get; } = "sub";
    public string NameClaimType { get; } = "name";
    public string RoleClaimType { get; } = "role";
    public string AccessTokenCookieName { get; } = "AccessToken";
    public string RefreshTokenCookieName { get; } = "RefreshToken";
}
