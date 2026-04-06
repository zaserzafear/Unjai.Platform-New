namespace Unjai.Platform.Domain.Entities.TenantsAdminRefreshToken;

public sealed record RefreshTokenCreationResult(
    string PlainToken,
    long Expires
);
