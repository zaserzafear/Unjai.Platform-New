using Unjai.Platform.Domain.Entities.TenantsAdmin;

namespace Unjai.Platform.Application.Repositories.TenantAdmins;

public interface ITenantAdminRepository
{
    Task<bool> HasDefaultAdminAsync(CancellationToken ct);
    Task<TenantAdmin> CreateAsync(TenantAdmin tenantAdmin, CancellationToken ct);
    Task<TenantAdmin?> LoginAsync(string username, string password, CancellationToken ct);
}
