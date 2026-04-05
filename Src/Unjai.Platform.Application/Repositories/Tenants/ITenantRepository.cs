using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Domain.Entities.Tenants;

namespace Unjai.Platform.Application.Repositories.Tenants;

public interface ITenantRepository
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken ct = default);
    Task CreateAsync(Tenant tenant, CancellationToken ct = default);
    Task<PagedResult<Tenant>> GetAllAsync(
        int page,
        int pageSize,
        CancellationToken ct = default);
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct = default);
    void Update(Tenant tenant);
    void Remove(Tenant tenant);
}
