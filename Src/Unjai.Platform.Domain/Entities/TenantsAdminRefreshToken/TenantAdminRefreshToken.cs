using Unjai.Platform.Domain.Entities.TenantsAdmin;

namespace Unjai.Platform.Domain.Entities.TenantsAdminRefreshToken;

public sealed class TenantAdminRefreshToken
{
    public Guid Id { get; set; }
    public Guid TenantAdminId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime CreatedAt { get; set; }

    public TenantAdmin TenantAdmin { get; set; } = null!;
}
