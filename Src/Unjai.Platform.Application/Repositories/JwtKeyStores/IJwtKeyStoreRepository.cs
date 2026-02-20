using Unjai.Platform.Domain.Entities.JwtSigningKeys;

namespace Unjai.Platform.Application.Repositories.JwtKeyStores;

public interface IJwtKeyStoreRepository
{
    void Add(JwtSigningKey key);
    JwtSigningKey GetActiveKey();
    IEnumerable<JwtSigningKey> GetAllPublicKeys();
    void RotateKey(TimeSpan keyLifetime);
}
