using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Unjai.Platform.Application.Services.TenantAdminRolePermissions;
using Unjai.Platform.Domain.Security.Principals;
using Unjai.Platform.Infrastructure.Security.Authentication.Jwt;

namespace Unjai.Platform.Infrastructure.Security.Authentication.Policies.TenantAdmins;

internal sealed class TenantAdminPermissionHandler : AuthorizationHandler<TenantAdminPermissionRequirement>
{
    private readonly TenantAdminRolePermissionService _service;
    private readonly JwtSettings _jwtSettings;

    public TenantAdminPermissionHandler(
        TenantAdminRolePermissionService service,
        IOptions<JwtSettings> jwtSettings)
    {
        _service = service;
        _jwtSettings = jwtSettings.Value;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        TenantAdminPermissionRequirement requirement)
    {
        var principalType = context.User.FindFirstValue(SecurityClaimTypes.PrincipalType);
        if (!string.Equals(principalType, PrincipalType.TenantAdmin, StringComparison.Ordinal))
            return;

        var roleClaim = context.User.FindFirstValue(_jwtSettings.RoleClaimType);
        if (!int.TryParse(roleClaim, NumberStyles.None, CultureInfo.InvariantCulture, out var roleId))
            return;

        var permissions = await _service.GetByRoleId(roleId, CancellationToken.None);

        if (permissions.Any(p => p.PermissionId == requirement.PermissionId))
        {
            context.Succeed(requirement);
        }
    }
}
