using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Abstractions.Security.Authentication;
using Unjai.Platform.Application.Diagnostics;
using Unjai.Platform.Application.Repositories.TenantAdmins;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.TenantAdmins;

namespace Unjai.Platform.Application.Services.TenantAdmins.LoginTenantAdmin;

public sealed class LoginTenantAdminV1(
    ILogger<LoginTenantAdminV1> logger,
    ITenantAdminRepository repository,
    ITokenProvider jwtTokenIssuer,
    ActivitySource activitySource)
{
    public async Task<AppResult<LoginTenantAdminResponseDto>> Handle(LoginTenantAdminRequestDto request, CancellationToken ct)
    {
        using var activity = activitySource.StartMethodActivity(typeof(LoginTenantAdminV1));

        activity?.SetTag("service", nameof(LoginTenantAdminV1));
        activity?.SetTag("operation", nameof(Handle));
        activity?.SetTag("auth.username", request.Username);

        try
        {
            var tenantAdmin = await repository.LoginAsync(request.Username, request.Password, ct);

            activity?.SetTag("auth.user_found", tenantAdmin is not null);

            if (tenantAdmin is null)
            {
                activity?.SetTag("auth.login.result", "failed");
                activity?.SetTag("auth.failure_reason", "invalid_credentials");
                activity?.SetStatus(ActivityStatusCode.Ok);

                return AppResult<LoginTenantAdminResponseDto>.Fail(
                    httpStatus: 401,
                    statusCode: "INVALID_CREDENTIALS",
                    message: "Invalid username or password."
                );
            }

            using var tokenActivity = activitySource.StartActivity("auth.jwt.issue");

            tokenActivity?.SetTag("service", nameof(LoginTenantAdminV1));
            tokenActivity?.SetTag("operation", "IssueAccessToken");
            tokenActivity?.SetTag("auth.username", request.Username);

            try
            {
                var accessToken = await jwtTokenIssuer.IssueAccessToken(tenantAdmin, ct);

                tokenActivity?.SetTag("auth.token.type", "access");
                tokenActivity?.SetTag("auth.token.expires_at", accessToken.expires);
                tokenActivity?.SetStatus(ActivityStatusCode.Ok);

                activity?.SetTag("auth.login.result", "success");
                activity?.SetTag("auth.token.type", "access");
                activity?.SetTag("auth.token.expires_at", accessToken.expires);
                activity?.SetStatus(ActivityStatusCode.Ok);

                return AppResult<LoginTenantAdminResponseDto>.Ok(
                    httpStatus: 200,
                    statusCode: "LOGIN_SUCCESSFUL",
                    message: "Tenant admin logged in successfully.",
                    data: new LoginTenantAdminResponseDto(
                        accessToken.token,
                        accessToken.expires,
                        string.Empty)
                );
            }
            catch (Exception ex)
            {
                tokenActivity?.SetTag("error", true);
                tokenActivity?.SetTag("error.type", ex.GetType().FullName);
                tokenActivity?.SetTag("error.message", ex.Message);
                tokenActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);

                throw;
            }
        }
        catch (Exception ex)
        {
            activity?.SetTag("error", true);
            activity?.SetTag("error.type", ex.GetType().FullName);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            logger.LogError(
                ex,
                "An error occurred while logging in tenant admin with username '{Username}'.",
                request.Username);

            return AppResult<LoginTenantAdminResponseDto>.Fail(
                httpStatus: 500,
                statusCode: "INTERNAL_SERVER_ERROR",
                message: "An error occurred while processing the login request."
            );
        }
    }
}
