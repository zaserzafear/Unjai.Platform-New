using Unjai.Platform.Domain.Entities.TenantsAdmin;
using Unjai.Platform.Domain.Entities.TenantsAdminRefreshToken;

namespace Unjai.Platform.Application.Repositories.TenantAdmins;

public interface ITenantAdminRepository
{
    Task<bool> HasDefaultAdminAsync(CancellationToken ct = default);
    Task<TenantAdmin> CreateAsync(TenantAdmin tenantAdmin, CancellationToken ct = default);
    Task<TenantAdmin?> LoginAsync(string username, string password, CancellationToken ct = default);
    Task<RefreshTokenCreationResult> AddRefreshTokenAsync(Guid tenantAdminId, int expireDays = 7, CancellationToken ct = default);
}
