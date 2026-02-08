namespace Unjai.Platform.Contracts.Tenants.Dtos;

public sealed record GetTenantResponseDto(
    Guid Id,
    string Code,
    string Name,
    bool IsActive,
    DateTime CreatedAt,
    DateTime UpdatedAt);
