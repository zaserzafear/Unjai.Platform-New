namespace Unjai.Platform.Contracts.Tenants;

public sealed record TenantCreateRequestDto(
    string Code,
    string Name);

public sealed record CreateTenantRequestValidationErrorDto(
    string Code,
    string Message
);
