using Microsoft.EntityFrameworkCore;
using Unjai.Platform.Application.Repositories.TenantAdmins;
using Unjai.Platform.Domain.Entities.TenantsAdmin;
using Unjai.Platform.Domain.Entities.TenantsAdminRefreshToken;
using Unjai.Platform.Infrastructure.Persistent.Database;
using Unjai.Platform.Infrastructure.Security.Authentication.RefreshToken;
using Unjai.Platform.Infrastructure.Security.Cryptography.Hashing;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.TenantAdmins;

internal sealed class TenantAdminRepository(
    WriteDbContext writeDbContext,
    ReadDbContext readDbContext) : ITenantAdminRepository
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

        if (tenantAdmin is null)
        {
            return null;
        }

        bool isValid = PasswordHasher.Verify(password, tenantAdmin.PasswordHash);

        if (!isValid)
        {
            return null;
        }

        return tenantAdmin;
    }

    public async Task<RefreshTokenCreationResult> AddRefreshTokenAsync(
        Guid tenantAdminId,
        int expireDays = 7,
        CancellationToken ct = default)
    {
        string plainToken = string.Empty;
        string tokenHash = string.Empty;
        bool isUnique = false;

        while (!isUnique)
        {
            plainToken = RefreshTokenProtector.GenerateToken();
            tokenHash = RefreshTokenProtector.HashToken(plainToken);

            bool isExists = await writeDbContext.TenantAdminRefreshTokens
                .AnyAsync(x => x.TokenHash == tokenHash, ct);

            if (!isExists)
            {
                isUnique = true;
            }
        }

        var refreshToken = new TenantAdminRefreshToken
        {
            TenantAdminId = tenantAdminId,
            TokenHash = tokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(expireDays),
            IsRevoked = false,
            CreatedAt = DateTime.UtcNow
        };

        await writeDbContext.TenantAdminRefreshTokens.AddAsync(refreshToken, ct);

        return new RefreshTokenCreationResult(
            PlainToken: plainToken,
            Expires: new DateTimeOffset(refreshToken.ExpiresAt).ToUnixTimeSeconds());
    }
}
