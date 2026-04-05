using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;
using Unjai.Platform.Application.Repositories.TenantAdmins;
using Unjai.Platform.Domain.Entities.TenantsAdmin;
using Unjai.Platform.Domain.Entities.TenantsAdminRefreshToken;
using Unjai.Platform.Infrastructure.Persistent.Database;
using Unjai.Platform.Infrastructure.Security.Cryptography.Hashing;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.TenantAdmins;

internal sealed class TenantAdminRepository(WriteDbContext writeDbContext, ReadDbContext readDbContext) : ITenantAdminRepository
{
    public async Task<bool> HasDefaultAdminAsync(CancellationToken ct = default)
    {
        bool hasAdmin = await readDbContext
            .TenantAdmins
            .AnyAsync(ct);

        return hasAdmin;
    }

    public async Task<TenantAdmin> CreateAsync(TenantAdmin tenantAdmin, CancellationToken ct = default)
    {
        await writeDbContext.TenantAdmins.AddAsync(tenantAdmin, ct);

        return tenantAdmin;
    }

    public async Task<TenantAdmin?> LoginAsync(string username, string password, CancellationToken ct = default)
    {
        var tenantAdmin = await readDbContext
            .TenantAdmins
            .Where(t => t.Username == username)
            .Select(x => new TenantAdmin
            {
                Id = x.Id,
                Username = x.Username,
                PasswordHash = x.PasswordHash,
                RoleId = x.RoleId,
            })
            .SingleOrDefaultAsync(ct);

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

    public async Task<TenantAdminRefreshToken> AddRefreshTokenAsync(Guid tenantAdminId, int expireDays = 7, CancellationToken ct = default)
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);

        var refreshToken = new TenantAdminRefreshToken
        {
            TenantAdminId = tenantAdminId,
            Token = Convert.ToBase64String(randomNumber),
            ExpiresAt = DateTime.UtcNow.AddDays(expireDays),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await writeDbContext.TenantAdminRefreshTokens.AddAsync(refreshToken, ct);

        return refreshToken;
    }
}
