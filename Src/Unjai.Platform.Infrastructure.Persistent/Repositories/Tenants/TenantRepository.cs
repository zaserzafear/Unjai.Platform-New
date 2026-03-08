using Microsoft.EntityFrameworkCore;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Domain.Entities.Tenants;
using Unjai.Platform.Infrastructure.Persistent.Database;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.Tenants;

internal sealed class TenantRepository(
    WriteDbContext writeDb,
    ReadDbContext readDb)
    : ITenantRepository
{
    public Task<bool> ExistsByCodeAsync(string code, CancellationToken ct)
    {
        return readDb.Tenants
            .IgnoreQueryFilters()
            .AnyAsync(t => t.Code == code, ct);
    }

    public Task CreateAsync(Tenant tenant, CancellationToken ct)
    {
        return writeDb.Tenants.AddAsync(tenant, ct)
            .AsTask();
    }

    public async Task<PagedResult<Tenant>> GetAllAsync(
        int page,
        int pageSize,
        CancellationToken ct)
    {
        var query = readDb.Tenants
            .AsNoTracking()
            .OrderBy(t => t.CreatedAt);

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PagedResult<Tenant>(
            Items: items,
            Page: page,
            PageSize: pageSize,
            TotalCount: totalCount
        );
    }

    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        return readDb.Tenants
            .Where(t => t.Id == id)
            .FirstOrDefaultAsync(ct);
    }

    public void Update(Tenant tenant)
    {
        writeDb.Tenants.Update(tenant);
    }

    public void Remove(Tenant tenant)
    {
        writeDb.Tenants.Remove(tenant);
    }
}
