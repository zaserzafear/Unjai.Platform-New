using Microsoft.EntityFrameworkCore;
using Unjai.Platform.Application.Repositories.JwtKeyStores;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;
using Unjai.Platform.Infrastructure.Persistent.Database;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.JwtSigningKeys;

internal sealed class JwtKeyStoreRepository(
    WriteDbContext writeDbContext,
    ReadDbContext readDbContext) : IJwtKeyStoreRepository
{
    public async Task<JwtSigningKey> CreateAsync(JwtSigningKey jwtSigningKey)
    {
        await writeDbContext.JwtSigningKeys.AddAsync(jwtSigningKey);

        return jwtSigningKey;
    }

    public async Task<JwtSigningKey?> GetActiveNotExpiredKey(CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        return await readDbContext.JwtSigningKeys
            .SingleOrDefaultAsync(
            x => x.IsActive &&
            x.ExpiresAt > now,
            ct);
    }

    public async Task<IEnumerable<JwtSigningKey>> GetAllNotExpiredKeysAsync(CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        return await readDbContext.JwtSigningKeys
            .Where(x => x.ExpiresAt > now)
            .ToListAsync(ct);
    }

    public JwtSigningKey Update(JwtSigningKey jwtSigningKey)
    {
        writeDbContext.JwtSigningKeys.Update(jwtSigningKey);

        return jwtSigningKey;
    }
}
