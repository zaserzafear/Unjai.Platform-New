using Unjai.Platform.Application.Repositories.JwtKeyStores;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;
using Unjai.Platform.Infrastructure.Persistent.Database;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.JwtSigningKeys;

internal sealed class JwtKeyStoreRepository(
    WriteDbContext writeDbContext,
    ReadDbContext readDbContext) : IJwtKeyStoreRepository
{
    public async Task<JwtSigningKey> AddAsync(JwtSigningKey jwtSigningKey)
    {
        await writeDbContext.JwtSigningKeys.AddAsync(jwtSigningKey);

        return jwtSigningKey;
    }

    public JwtSigningKey? GetActiveNotExpiredKey()
    {
        var now = DateTime.UtcNow;

        return readDbContext.JwtSigningKeys
            .SingleOrDefault(x => x.IsActive && x.ExpiresAt > now);
    }

    public IEnumerable<JwtSigningKey> GetAllNotExpiredKeys()
    {
        var now = DateTime.UtcNow;

        return readDbContext.JwtSigningKeys
            .Where(x => x.ExpiresAt > now)
            .ToList();
    }

    public JwtSigningKey Update(JwtSigningKey jwtSigningKey)
    {
        writeDbContext.JwtSigningKeys.Update(jwtSigningKey);

        return jwtSigningKey;
    }
}
