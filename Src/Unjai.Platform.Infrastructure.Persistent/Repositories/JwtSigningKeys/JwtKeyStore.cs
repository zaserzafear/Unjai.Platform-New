using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;
using Unjai.Platform.Infrastructure.Persistent.Database;
using Unjai.Platform.Infrastructure.Security.Authentication.Jwt;
using Unjai.Platform.Infrastructure.Security.Cryptography.Ecdsa;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories.JwtSigningKeys;

internal sealed class JwtKeyStore(WriteDbContext dbContext) : IJwtKeyStore
{
    public void Add(JwtSigningKey key)
    {
        dbContext.JwtSigningKeys.Add(key);
        dbContext.SaveChanges();
    }

    public JwtSigningKey GetActiveKey()
    {
        return dbContext.JwtSigningKeys.AsNoTracking().Single(x => x.IsActive);
    }

    public SecurityKey GetActiveSecurityKey()
    {
        return GetActiveKey().ToSecurityKey();
    }

    public IEnumerable<JwtSigningKey> GetAllPublicKeys()
    {
        return dbContext.JwtSigningKeys.AsNoTracking().ToList();
    }

    public IEnumerable<SecurityKey> GetAllPublicSecurityKeys()
    {
        return dbContext.JwtSigningKeys
            .AsNoTracking()
            .Select(x => x.ToSecurityKey())
            .ToList();
    }

    public IEnumerable<SecurityKey> GetAllSecurityKeys()
    {
        return GetAllPublicKeys().Select(x => x.ToSecurityKey());
    }

    public void RotateKey()
    {
        var ecdsaKey = EcdsaKeyGenerator.Create();

        var newKey = new JwtSigningKey
        {
            KeyId = ecdsaKey.kid,
            PrivateKeyPem = ecdsaKey.privatePem,
            PublicKeyPem = ecdsaKey.publicPem,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
        };

        var activeKey = dbContext.JwtSigningKeys.AsNoTracking().SingleOrDefault(x => x.IsActive);

        if (activeKey != null)
        {
            activeKey.IsActive = false;
            dbContext.JwtSigningKeys.Update(activeKey);
        }

        dbContext.JwtSigningKeys.Add(newKey);
        dbContext.SaveChanges();
    }
}
