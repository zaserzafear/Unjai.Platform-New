using Microsoft.IdentityModel.Tokens;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;

namespace Unjai.Platform.Infrastructure.Security.Authentication.Jwt;

public interface IJwtKeyStore
{
    void Add(JwtSigningKey key);
    JwtSigningKey GetActiveKey();
    IEnumerable<JwtSigningKey> GetAllPublicKeys();
    IEnumerable<SecurityKey> GetAllSecurityKeys();
    SecurityKey GetActiveSecurityKey();
    void RotateKey();
}
