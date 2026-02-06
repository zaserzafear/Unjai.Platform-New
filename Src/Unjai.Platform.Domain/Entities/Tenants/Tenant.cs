using Unjai.Platform.Domain.Entities.Abstractions;

namespace Unjai.Platform.Domain.Entities.Tenants;

public sealed class Tenant : EntityBase
{
    public Guid Id { get; set; }
    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
