namespace Unjai.Platform.Contracts.TenantAdmins.Dtos;

public sealed record LoginTenantAdminResponseDto(string AccessToken, long AccessTokenExpires, string RefreshToken);
