using Unjai.Platform.Domain.Entities.Tenants;

namespace Unjai.Platform.Application.Repositories.Tenants;

public interface ITenantRepository
{
    Task CreateAsync(Tenant tenant, CancellationToken ct);
    Task<List<Tenant>> GetAllAsync(int page, int pageSize, CancellationToken ct);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken ct);
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct);
    void Update(Tenant tenant);
}
