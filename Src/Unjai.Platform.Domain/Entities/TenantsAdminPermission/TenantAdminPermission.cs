namespace Unjai.Platform.Domain.Entities.TenantsAdminPermission;

public sealed class TenantAdminPermission
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;
    public string Name { get; set; } = null!;
}
