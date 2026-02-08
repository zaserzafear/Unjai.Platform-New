using Microsoft.EntityFrameworkCore;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Domain.Entities.Tenants;
using Unjai.Platform.Infrastructure.Persistent.Database;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.Tenants;

internal sealed class TenantRepository(
    WriteDbContext writeDb,
    ReadDbContext readDb)
    : ITenantRepository
{
    public Task CreateAsync(Tenant tenant, CancellationToken ct)
        => writeDb.Tenants.AddAsync(tenant, ct).AsTask();

    public Task<List<Tenant>> GetAllAsync(int page, int pageSize, CancellationToken ct)
        => readDb.Tenants
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
            .ToListAsync(ct);

    public Task<bool> ExistsByCodeAsync(string code, CancellationToken ct)
        => readDb.Tenants.AnyAsync(t => t.Code == code, ct);

    public Task<Tenant?> GetByIdAsync(Guid id, CancellationToken ct)
        => readDb.Tenants
            .Where(t => t.Id == id)
            .Select(t => new Tenant
            {
                Id = t.Id,
                Code = t.Code,
                Name = t.Name,
                IsActive = t.IsActive,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt,
            })
            .FirstOrDefaultAsync(ct);

    public void Update(Tenant tenant)
        => writeDb.Tenants.Update(tenant);
}
