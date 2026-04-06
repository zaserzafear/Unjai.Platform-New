using Unjai.Platform.Domain.Entities.TenantsAdmin;

namespace Unjai.Platform.Application.Abstractions.Security.Authentication;

public interface ITokenProvider
{
    Task<(string Token, long Expires)> IssueAccessToken(TenantAdmin entity, CancellationToken ct);
    (string Token, string TokenHash, DateTime Expires) IssueRefreshToken(Guid prefix, CancellationToken ct);
}
