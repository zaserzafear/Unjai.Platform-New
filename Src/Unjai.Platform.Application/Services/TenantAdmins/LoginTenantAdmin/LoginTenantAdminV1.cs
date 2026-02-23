using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Repositories.TenantAdmins;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.TenantAdmins.Dtos;

namespace Unjai.Platform.Application.Services.TenantAdmins.LoginTenantAdmin;

public sealed class LoginTenantAdminV1(
     ILogger<LoginTenantAdminV1> logger,
     ITenantAdminRepository repository)
{
    public async Task<AppResult<LoginTenantAdminResponseDto>> Handle(LoginTenantAdminRequestDto request, CancellationToken ct)
    {
        try
        {
            var tenantAdmin = await repository.LoginAsync(request.Username, request.Password, ct);
            if (tenantAdmin is null)
            {
                return AppResult<LoginTenantAdminResponseDto>.Fail(
                    httpStatus: 401,
                    statusCode: "INVALID_CREDENTIALS",
                    message: "Invalid username or password."
                );
            }
            return AppResult<LoginTenantAdminResponseDto>.Ok(
                httpStatus: 200,
                statusCode: "LOGIN_SUCCESSFUL",
                message: "Tenant admin logged in successfully."
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while logging in tenant admin with username '{Username}'", request.Username);
            return AppResult<LoginTenantAdminResponseDto>.Fail(
                httpStatus: 500,
                statusCode: "INTERNAL_SERVER_ERROR",
                message: "An error occurred while processing the login request."
            );
        }
    }
}
