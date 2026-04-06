namespace Unjai.Platform.Contracts.TenantAdmins;

public record class TenantAdminLoginRequestDto(string Username, string Password);

public sealed record TenantAdminLoginResponseDto(
    string AccessToken,
    long AccessTokenExpires,
    string RefreshToken,
    long RefreshTokenExpires);
