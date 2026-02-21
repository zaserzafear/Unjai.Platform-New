using Unjai.Platform.Domain.Entities.JwtSigningKeys;

namespace Unjai.Platform.Application.Repositories.JwtKeyStores;

public interface IJwtKeyStoreRepository
{
    Task<JwtSigningKey> AddAsync(JwtSigningKey jwtSigningKey);
    JwtSigningKey? GetActiveNotExpiredKey();
    public IEnumerable<JwtSigningKey> GetAllNotExpiredKeys();
    JwtSigningKey Update(JwtSigningKey jwtSigningKey);
}
