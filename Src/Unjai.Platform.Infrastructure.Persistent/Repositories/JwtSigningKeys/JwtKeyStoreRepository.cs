using Microsoft.EntityFrameworkCore;
using Unjai.Platform.Application.Repositories.JwtKeyStores;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;
using Unjai.Platform.Infrastructure.Persistent.Database;
using Unjai.Platform.Infrastructure.Security.Cryptography.Ecdsa;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.JwtSigningKeys;

internal sealed class JwtKeyStoreRepository(WriteDbContext dbContext) : IJwtKeyStoreRepository
{
    public void Add(JwtSigningKey key)
    {
        dbContext.JwtSigningKeys.Add(key);
        dbContext.SaveChanges();
    }

    public JwtSigningKey GetActiveKey()
    {
        var now = DateTime.UtcNow;

        var key = dbContext.JwtSigningKeys
            .AsNoTracking()
            .SingleOrDefault(x => x.IsActive && x.ExpiresAt > now);

        if (key == null)
            throw new InvalidOperationException("No active JWT signing key available.");

        return key;
    }

    public IEnumerable<JwtSigningKey> GetAllPublicKeys()
    {
        var now = DateTime.UtcNow;

        return dbContext.JwtSigningKeys
            .AsNoTracking()
            .Where(x => x.ExpiresAt > now)
            .ToList();
    }

    public void RotateKey(TimeSpan keyLifetime)
    {
        var now = DateTime.UtcNow;

        var activeKey = dbContext.JwtSigningKeys
            .SingleOrDefault(x => x.IsActive);

        if (activeKey != null)
        {
            activeKey.IsActive = false;
            dbContext.JwtSigningKeys.Update(activeKey);
        }

        var ecdsaKey = EcdsaKeyGenerator.Create();

        var newKey = new JwtSigningKey
        {
            KeyId = ecdsaKey.kid,
            PrivateKeyPem = ecdsaKey.privatePem,
            PublicKeyPem = ecdsaKey.publicPem,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = now.Add(keyLifetime),
        };

        dbContext.JwtSigningKeys.Add(newKey);
    }
}
