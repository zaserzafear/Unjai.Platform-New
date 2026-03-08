namespace Unjai.Platform.Contracts.Tenants;

public sealed record CreateTenantRequestDto(
    string Code,
    string Name);

public sealed record CreateTenantRequestValidationErrorDto(
    string Code,
    string Message
);
