using Microsoft.EntityFrameworkCore;
using Unjai.Platform.Application.Repositories.TenantAdmins;
using Unjai.Platform.Domain.Entities.TenantsAdmin;
using Unjai.Platform.Infrastructure.Persistent.Database;
using Unjai.Platform.Infrastructure.Security.Cryptography.Hashing;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.TenantAdmins;

internal sealed class TenantAdminRepository(WriteDbContext writeDbContext, ReadDbContext readDbContext) : ITenantAdminRepository
{
    public async Task<bool> HasDefaultAdminAsync(CancellationToken ct)
    {
        bool hasAdmin = await readDbContext
            .TenantAdmins
            .AnyAsync(ct);

        return hasAdmin;
    }

    public async Task<TenantAdmin> CreateAsync(TenantAdmin tenantAdmin, CancellationToken ct)
    {
        await writeDbContext.TenantAdmins.AddAsync(tenantAdmin, ct);

        return tenantAdmin;
    }

    public async Task<TenantAdmin?> LoginAsync(string username, string password, CancellationToken ct)
    {
        var tenantAdmin = await readDbContext
            .TenantAdmins
            .SingleOrDefaultAsync(x => x.Username == username, ct);

        if (tenantAdmin == null)
        {
            return null;
        }

        bool isValid = PasswordHasher.Verify(
            password,
            tenantAdmin.PasswordHash
        );

        if (!isValid)
            return null;

        return tenantAdmin;
    }
}
