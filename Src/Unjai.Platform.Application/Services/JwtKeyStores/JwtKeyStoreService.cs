using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Abstractions.Caching;
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
    HybridCache cache)
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
                    return repository.GetAllPublicKeys();
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
            var activeKey = repository.GetActiveKey();

            var cacheKeyAllPublicKeys = JwtKeyStoreCacheKeys.GetAllPublicKeys;
            var cacheKeyActiveKey = JwtKeyStoreCacheKeys.GetByKid(activeKey.KeyId);

            repository.RotateKey(keyLifetime);

            await unitOfWork.SaveChangesAsync(ct);

            await cacheInvalidation.NotifyCacheInvalidationAsync(cacheKeyAllPublicKeys);
            await cacheInvalidation.NotifyCacheInvalidationAsync(cacheKeyActiveKey);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to rotate JWT signing keys.");
            throw;
        }
    }
}
