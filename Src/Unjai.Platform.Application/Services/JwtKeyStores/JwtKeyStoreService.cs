using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Abstractions.Caching;
using Unjai.Platform.Application.Abstractions.Cryptography.Ecdsa;
using Unjai.Platform.Application.Repositories.JwtKeyStores;
using Unjai.Platform.Domain.Abstractions;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;

namespace Unjai.Platform.Application.Services.JwtKeyStores;

public interface IJwtKeyStoreService
{
    Task<IEnumerable<JwtSigningKey>> GetAllPublicKeysAsync();
    Task RotateKeyAsync(TimeSpan keyLifetime, CancellationToken ct);
}

internal sealed class JwtKeyStoreService(
    ILogger<JwtKeyStoreService> logger,
    IUnitOfWork unitOfWork,
    IJwtKeyStoreRepository repository,
    ICacheInvalidationPublisherService cacheInvalidation,
    HybridCache cache,
    IEcdsaKeyGenerator ecdsaKeyGenerator)
    : IJwtKeyStoreService
{
    public async Task<IEnumerable<JwtSigningKey>> GetAllPublicKeysAsync()
    {
        try
        {
            var cacheKey = JwtKeyStoreCacheKeys.GetAllPublicKeys;

            var publicKeys = await cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    return repository.GetAllNotExpiredKeys();
                });

            return publicKeys;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve JWT signing keys from the repository.");
            throw;
        }
    }

    public async Task RotateKeyAsync(TimeSpan keyLifetime, CancellationToken ct)
    {
        try
        {
            var now = DateTime.UtcNow;

            var activeKey = repository.GetActiveNotExpiredKey();

            if (activeKey is not null)
            {
                activeKey.IsActive = false;
                repository.Update(activeKey);
            }

            var (privatePem, publicPem, kid) = ecdsaKeyGenerator.Create();

            var newKey = new JwtSigningKey
            {
                KeyId = kid,
                PrivateKeyPem = privatePem,
                PublicKeyPem = publicPem,
                IsActive = true,
                CreatedAt = now,
                ExpiresAt = now.Add(keyLifetime),
            };

            await repository.AddAsync(newKey);

            await unitOfWork.SaveChangesAsync(ct);

            await cacheInvalidation.NotifyCacheInvalidationAsync(
                JwtKeyStoreCacheKeys.GetAllPublicKeys);

            if (activeKey is not null)
            {
                await cacheInvalidation.NotifyCacheInvalidationAsync(
                    JwtKeyStoreCacheKeys.GetByKid(activeKey.KeyId));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to rotate JWT signing keys.");
            throw;
        }
    }
}
