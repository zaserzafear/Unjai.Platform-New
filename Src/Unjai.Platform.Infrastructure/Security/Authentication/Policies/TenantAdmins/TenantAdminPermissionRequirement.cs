using Microsoft.AspNetCore.Authorization;

namespace Unjai.Platform.Infrastructure.Security.Authentication.Policies.TenantAdmins;

internal sealed class TenantAdminPermissionRequirement : IAuthorizationRequirement
{
    public int PermissionId { get; }

    public TenantAdminPermissionRequirement(int permissionId)
    {
        PermissionId = permissionId;
    }
}
