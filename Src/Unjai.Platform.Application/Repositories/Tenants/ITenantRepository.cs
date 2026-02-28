using Unjai.Platform.Domain.Entities.Tenants;

namespace Unjai.Platform.Application.Repositories.Tenants;

public interface ITenantRepository
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken ct);
    Task CreateAsync(Tenant tenant, CancellationToken ct);
    Task<IReadOnlyList<Tenant>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct);
    void Update(Tenant tenant);
    void Remove(Tenant tenant);
}
