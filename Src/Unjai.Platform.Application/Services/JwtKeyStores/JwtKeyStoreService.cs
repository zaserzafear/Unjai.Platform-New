using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Abstractions.Caching;
using Unjai.Platform.Application.Abstractions.Security.Cryptography.Ecdsa;
using Unjai.Platform.Application.Repositories.JwtKeyStores;
using Unjai.Platform.Domain.Abstractions;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;

namespace Unjai.Platform.Application.Services.JwtKeyStores;

public sealed class JwtKeyStoreService(
    ILogger<JwtKeyStoreService> logger,
    IUnitOfWork unitOfWork,
    IJwtKeyStoreRepository repository,
    ICacheInvalidationPublisherService cacheInvalidation,
    HybridCache cache,
    IEcdsaKeyGenerator ecdsaKeyGenerator)
{
    public async Task<JwtSigningKey?> GetActiveNotExpiredKey(CancellationToken ct)
    {
        try
        {
            var cacheKey = JwtKeyStoreCacheKeys.GetActiveKeys;
            var activeKey = await cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
                    {
                        return await repository.GetActiveNotExpiredKey(ct);
                    }, cancellationToken: ct);
            return activeKey;

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve the active JWT signing key from the repository.");
            throw;
        }
    }

    public IEnumerable<JwtSigningKey> GetAllPublicKeys()
    {
        try
        {
            var cacheKey = JwtKeyStoreCacheKeys.GetAllPublicKeys;

            var publicKeys = cache.GetOrCreateAsync(
                cacheKey,
                async ct =>
                {
                    return await repository.GetAllNotExpiredKeysAsync(ct);
                })
                .AsTask()
                .GetAwaiter()
                .GetResult();

            return publicKeys;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve JWT signing keys from the repository.");
            throw;
        }
    }

    private async Task RotateKeyAsync(TimeSpan keyLifetime, CancellationToken ct)
    {
        try
        {
            var now = DateTime.UtcNow;

            var activeKey = await repository.GetActiveNotExpiredKey(ct);

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

            await repository.CreateAsync(newKey);

            await unitOfWork.SaveChangesAsync(ct);

            await cacheInvalidation.NotifyCacheInvalidationAsync(
                JwtKeyStoreCacheKeys.GetAllPublicKeys);

            await cacheInvalidation.NotifyCacheInvalidationAsync(
                JwtKeyStoreCacheKeys.GetActiveKeys);

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

    public async Task<bool> RotateKeyIfNeededAsync(
        TimeSpan rotateBeforeExpiry,
        TimeSpan keyLifetime,
        CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var activeKey = await repository.GetActiveNotExpiredKey(ct);

        if (activeKey is null)
        {
            logger.LogWarning("No active JWT signing key found. Rotation required.");
            await RotateKeyAsync(keyLifetime, ct);
            return true;
        }

        var remaining = activeKey.ExpiresAt - now;

        if (remaining <= rotateBeforeExpiry)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                    "JWT signing key {KeyId} will expire in {Remaining}. Rotating.",
                    activeKey.KeyId,
                    remaining);
            }

            await RotateKeyAsync(keyLifetime, ct);
            return true;
        }

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(
                "JWT signing key {KeyId} is still valid for {Remaining}. Rotation skipped.",
                activeKey.KeyId,
                remaining);
        }

        return false;
    }
}
