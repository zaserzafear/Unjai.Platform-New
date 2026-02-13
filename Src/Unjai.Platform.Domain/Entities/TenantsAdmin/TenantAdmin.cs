using Unjai.Platform.Domain.Abstractions;
using Unjai.Platform.Domain.Entities.TenantsAdminRole;

namespace Unjai.Platform.Domain.Entities.TenantsAdmin;

public sealed class TenantAdmin : EntityBase
{
    public string Username { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public bool IsActive { get; set; }

    public int RoleId { get; set; }
    public TenantAdminRole Role { get; set; } = null!;
}
