using System.Diagnostics;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Abstractions.Caching;
using Unjai.Platform.Application.Abstractions.Security.Cryptography.Ecdsa;
using Unjai.Platform.Application.Diagnostics;
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
    IEcdsaKeyGenerator ecdsaKeyGenerator,
    ActivitySource activitySource)
{
    public async Task<JwtSigningKey?> GetActiveNotExpiredKey(CancellationToken ct)
    {
        using var activity = activitySource.StartMethodActivity(typeof(JwtKeyStoreService));

        activity?.SetTag("service", nameof(JwtKeyStoreService));
        activity?.SetTag("operation", nameof(GetActiveNotExpiredKey));

        try
        {
            const string cacheName = nameof(JwtKeyStoreCacheKeys.GetActiveKeys);
            var cacheKey = JwtKeyStoreCacheKeys.GetActiveKeys;
            var cacheHit = true;

            activity?.SetTag("cache.name", cacheName);
            activity?.SetTag("cache.key", cacheKey);

            var activeKey = await cache.GetOrCreateAsync(
                cacheKey,
                async innerCt =>
                {
                    cacheHit = false;
                    return await repository.GetActiveNotExpiredKey(innerCt);
                },
                cancellationToken: ct);

            activity?.SetTag("cache.hit", cacheHit);
            activity?.SetTag("jwt.key.exists", activeKey is not null);
            activity?.SetTag("jwt.key.id", activeKey?.KeyId);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return activeKey;
        }
        catch (Exception ex)
        {
            activity?.SetTag("error", true);
            activity?.SetTag("error.type", ex.GetType().FullName);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            logger.LogError(ex, "Failed to retrieve active JWT signing key.");
            throw;
        }
    }

    public async Task<IEnumerable<JwtSigningKey>> GetAllNotExpiredKeysAsync()
    {
        using var activity = activitySource.StartMethodActivity(typeof(JwtKeyStoreService));

        activity?.SetTag("service", nameof(JwtKeyStoreService));
        activity?.SetTag("operation", nameof(GetAllNotExpiredKeysAsync));

        try
        {
            const string cacheName = nameof(JwtKeyStoreCacheKeys.GetAllPublicKeys);
            var cacheKey = JwtKeyStoreCacheKeys.GetAllPublicKeys;
            var cacheHit = true;

            activity?.SetTag("cache.name", cacheName);
            activity?.SetTag("cache.key", cacheKey);

            var publicKeys = await cache.GetOrCreateAsync(
                cacheKey,
                async _ =>
                {
                    cacheHit = false;
                    return await repository.GetAllNotExpiredKeysAsync(CancellationToken.None);
                });

            var publicKeysArray = publicKeys as JwtSigningKey[] ?? publicKeys.ToArray();

            activity?.SetTag("cache.hit", cacheHit);
            activity?.SetTag("jwt.keys.count", publicKeysArray.Length);
            activity?.SetStatus(ActivityStatusCode.Ok);

            return publicKeysArray;
        }
        catch (Exception ex)
        {
            activity?.SetTag("error", true);
            activity?.SetTag("error.type", ex.GetType().FullName);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            logger.LogError(ex, "Failed to retrieve JWT signing keys.");
            throw;
        }
    }

    private async Task RotateKeyAsync(TimeSpan keyLifetime, CancellationToken ct)
    {
        using var activity = activitySource.StartMethodActivity(typeof(JwtKeyStoreService));

        activity?.SetTag("service", nameof(JwtKeyStoreService));
        activity?.SetTag("operation", nameof(RotateKeyAsync));
        activity?.SetTag("jwt.key.lifetime.seconds", keyLifetime.TotalSeconds);

        try
        {
            var now = DateTime.UtcNow;
            var invalidatedCacheKeys = 0;

            activity?.SetTag("jwt.rotate.at_utc", now);

            var activeKey = await repository.GetActiveNotExpiredKey(ct);

            activity?.SetTag("jwt.old_key.exists", activeKey is not null);
            activity?.SetTag("jwt.old_key.id", activeKey?.KeyId);

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

            activity?.SetTag("jwt.new_key.id", kid);
            activity?.SetTag("jwt.new_key.expires_at_utc", newKey.ExpiresAt);

            await repository.CreateAsync(newKey);
            await unitOfWork.SaveChangesAsync(ct);

            await cacheInvalidation.NotifyCacheInvalidationAsync(JwtKeyStoreCacheKeys.GetAllPublicKeys);
            invalidatedCacheKeys++;

            await cacheInvalidation.NotifyCacheInvalidationAsync(JwtKeyStoreCacheKeys.GetActiveKeys);
            invalidatedCacheKeys++;

            if (activeKey is not null)
            {
                await cacheInvalidation.NotifyCacheInvalidationAsync(
                    JwtKeyStoreCacheKeys.GetByKid(activeKey.KeyId));
                invalidatedCacheKeys++;
            }

            activity?.SetTag("cache.invalidated.count", invalidatedCacheKeys);
            activity?.SetTag("jwt.rotation.result", "rotated");
            activity?.SetStatus(ActivityStatusCode.Ok);
        }
        catch (Exception ex)
        {
            activity?.SetTag("error", true);
            activity?.SetTag("error.type", ex.GetType().FullName);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            logger.LogError(ex, "Failed to rotate JWT signing keys.");
            throw;
        }
    }

    public async Task<bool> RotateKeyIfNeededAsync(
        TimeSpan rotateBeforeExpiry,
        TimeSpan keyLifetime,
        CancellationToken ct)
    {
        using var activity = activitySource.StartMethodActivity(typeof(JwtKeyStoreService));

        activity?.SetTag("service", nameof(JwtKeyStoreService));
        activity?.SetTag("operation", nameof(RotateKeyIfNeededAsync));
        activity?.SetTag("jwt.rotate_before_expiry.seconds", rotateBeforeExpiry.TotalSeconds);
        activity?.SetTag("jwt.key.lifetime.seconds", keyLifetime.TotalSeconds);

        try
        {
            var now = DateTime.UtcNow;

            activity?.SetTag("jwt.check.at_utc", now);

            var activeKey = await repository.GetActiveNotExpiredKey(ct);

            activity?.SetTag("jwt.active_key.exists", activeKey is not null);
            activity?.SetTag("jwt.active_key.id", activeKey?.KeyId);

            if (activeKey is null)
            {
                activity?.SetTag("jwt.rotation.reason", "missing_active_key");
                activity?.SetTag("jwt.rotation.result", "rotated");

                logger.LogWarning("No active JWT signing key found. Rotation required.");

                await RotateKeyAsync(keyLifetime, ct);

                activity?.SetStatus(ActivityStatusCode.Ok);
                return true;
            }

            var remaining = activeKey.ExpiresAt - now;

            activity?.SetTag("jwt.active_key.expires_at_utc", activeKey.ExpiresAt);
            activity?.SetTag("jwt.remaining.seconds", remaining.TotalSeconds);

            if (remaining <= rotateBeforeExpiry)
            {
                activity?.SetTag("jwt.rotation.reason", "expiring_soon");
                activity?.SetTag("jwt.rotation.result", "rotated");

                if (logger.IsEnabled(LogLevel.Information))
                {
                    logger.LogInformation(
                        "JWT signing key {KeyId} will expire in {Remaining}. Rotating.",
                        activeKey.KeyId,
                        remaining);
                }

                await RotateKeyAsync(keyLifetime, ct);

                activity?.SetStatus(ActivityStatusCode.Ok);
                return true;
            }

            activity?.SetTag("jwt.rotation.reason", "still_valid");
            activity?.SetTag("jwt.rotation.result", "skipped");
            activity?.SetStatus(ActivityStatusCode.Ok);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation(
                "JWT signing key {KeyId} is still valid for {Remaining}. Rotation skipped.",
                activeKey.KeyId,
                remaining);
            }

            return false;
        }
        catch (Exception ex)
        {
            activity?.SetTag("error", true);
            activity?.SetTag("error.type", ex.GetType().FullName);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            logger.LogError(ex, "Failed while checking JWT key rotation.");
            throw;
        }
    }
}
