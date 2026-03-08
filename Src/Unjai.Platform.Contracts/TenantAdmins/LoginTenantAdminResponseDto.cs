namespace Unjai.Platform.Contracts.TenantAdmins;

public sealed record LoginTenantAdminResponseDto(string AccessToken, long AccessTokenExpires, string RefreshToken);
