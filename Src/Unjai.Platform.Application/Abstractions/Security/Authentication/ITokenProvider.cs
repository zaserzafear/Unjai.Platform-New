using Unjai.Platform.Domain.Entities.TenantsAdmin;

namespace Unjai.Platform.Application.Abstractions.Security.Authentication;

public interface ITokenProvider
{
    Task<(string token, long expires)> IssueAccessToken(TenantAdmin entity, CancellationToken ct);
}
