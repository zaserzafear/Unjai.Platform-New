using Unjai.Platform.Domain.Entities.JwtSigningKeys;

namespace Unjai.Platform.Application.Repositories.JwtKeyStores;

public interface IJwtKeyStoreRepository
{
    Task<JwtSigningKey> CreateAsync(JwtSigningKey jwtSigningKey);
    Task<JwtSigningKey?> GetActiveNotExpiredKey(CancellationToken ct);
    Task<IEnumerable<JwtSigningKey>> GetAllNotExpiredKeysAsync(CancellationToken ct);
    JwtSigningKey Update(JwtSigningKey jwtSigningKey);
}
