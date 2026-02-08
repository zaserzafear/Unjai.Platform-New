using Microsoft.EntityFrameworkCore;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Domain.Entities.Tenants;
using Unjai.Platform.Infrastructure.Persistent.Database;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.Tenants;

internal sealed class TenantRepository(WriteDbContext writeDb, ReadDbContext readDb) : ITenantRepository
{
    public async Task<Tenant?> Create(Tenant tenant, CancellationToken cancellationToken)
    {
        await writeDb.Tenants.AddAsync(tenant, cancellationToken);
        await writeDb.SaveChangesAsync(cancellationToken);
        return tenant;
    }

    public async Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken)
    {
        return await readDb.Tenants
            .AnyAsync(t => t.Code == code, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await readDb.Tenants
            .AnyAsync(t => t.Name == name, cancellationToken);
    }

    public async Task<IReadOnlyList<Tenant>> GetAll(int page, int pageSize, CancellationToken cancellationToken)
    {
        return await readDb.Tenants
            .OrderBy(t => t.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new Tenant
            {
                Id = t.Id,
                Code = t.Code,
                Name = t.Name,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
            })
            .ToListAsync(cancellationToken);
    }
}
