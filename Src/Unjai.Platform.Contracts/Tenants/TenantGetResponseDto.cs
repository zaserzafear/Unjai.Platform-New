namespace Unjai.Platform.Contracts.Tenants;

public sealed record TenantGetResponseDto(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
