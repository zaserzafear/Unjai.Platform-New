using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Domain.Entities.Tenants;
using Unjai.Platform.Infrastructure.Messaging.Redis;
using Unjai.Platform.Infrastructure.Persistent.Database;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.Tenants;

internal sealed class TenantRepository(
    WriteDbContext writeDb,
    ReadDbContext readDb,
    HybridCache cache,
    IDistributedNotificationPublisher notificationPublisher)
    : ITenantRepository
{
    private static string CacheKeyById(Guid id) => $"TENANT_BY_ID_{id}";

    public async Task<Tenant?> CreateAsync(Tenant tenant, CancellationToken cancellationToken)
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

    public async Task<IReadOnlyList<Tenant>> GetAllAsync(int page, int pageSize, CancellationToken cancellationToken)
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

    public async Task<Tenant?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = CacheKeyById(id);

        return await cache.GetOrCreateAsync(cacheKey, async ct =>
                await readDb.Tenants
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
                    .FirstOrDefaultAsync(ct),
                    cancellationToken: cancellationToken);
    }

    public async Task<Tenant> UpdateAsync(Tenant tenant, CancellationToken cancellationToken)
    {
        writeDb.Tenants.Update(tenant);
        await writeDb.SaveChangesAsync(cancellationToken);

        var cacheKey = CacheKeyById(tenant.Id);
        await notificationPublisher.NotifyCacheInvalidationAsync(cacheKey);

        return tenant;
    }
}
