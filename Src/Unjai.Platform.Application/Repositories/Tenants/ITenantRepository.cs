using Unjai.Platform.Domain.Entities.Tenants;

namespace Unjai.Platform.Application.Repositories.Tenants;

public interface ITenantRepository
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task<Tenant?> CreateAsync(Tenant tenant, CancellationToken cancellationToken);
    Task<IReadOnlyList<Tenant>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken);
    Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Tenant> UpdateAsync(Tenant tenant, CancellationToken cancellationToken);
}
