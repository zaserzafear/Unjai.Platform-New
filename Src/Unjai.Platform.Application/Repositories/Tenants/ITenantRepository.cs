using Unjai.Platform.Domain.Entities.Tenants;

namespace Unjai.Platform.Application.Repositories.Tenants;

public interface ITenantRepository
{
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task<Tenant?> Create(Tenant tenant, CancellationToken cancellationToken);
    Task<IReadOnlyList<Tenant>> GetAll(int page, int pageSize, CancellationToken cancellationToken);
}
