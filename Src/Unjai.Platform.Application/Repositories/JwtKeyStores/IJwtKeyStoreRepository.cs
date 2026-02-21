using Unjai.Platform.Domain.Entities.JwtSigningKeys;

namespace Unjai.Platform.Application.Repositories.JwtKeyStores;

public interface IJwtKeyStoreRepository
{
    Task<JwtSigningKey> AddAsync(JwtSigningKey jwtSigningKey);
    JwtSigningKey? GetActiveNotExpiredKey();
    Task<IEnumerable<JwtSigningKey>> GetAllNotExpiredKeysAsync(CancellationToken ct);
    JwtSigningKey Update(JwtSigningKey jwtSigningKey);
}
