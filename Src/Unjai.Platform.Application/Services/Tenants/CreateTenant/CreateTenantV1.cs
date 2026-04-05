using System.Diagnostics;
using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Diagnostics;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.Tenants;
using Unjai.Platform.Domain.Abstractions;
using Unjai.Platform.Domain.Entities.Tenants;

namespace Unjai.Platform.Application.Services.Tenants.CreateTenant;

public sealed class CreateTenantV1(
    ILogger<CreateTenantV1> logger,
    IUnitOfWork unitOfWork,
    ITenantRepository repository,
    ActivitySource activitySource)
{
    public async Task<AppResult<object>> Handle(CreateTenantRequestDto request, CancellationToken ct)
    {
        using var activity = activitySource.StartMethodActivity(typeof(CreateTenantV1));

        activity?.SetTag("service", nameof(CreateTenantV1));
        activity?.SetTag("operation", nameof(Handle));
        activity?.SetTag("tenant.code", request.Code);

        try
        {
            var errors = new List<CreateTenantRequestValidationErrorDto>();

            using var existsCheckActivity = activitySource.StartActivity("tenant.exists.check");
            try
            {
                existsCheckActivity?.SetTag("service", nameof(CreateTenantV1));
                existsCheckActivity?.SetTag("operation", "ExistsByCodeAsync");
                existsCheckActivity?.SetTag("tenant.code", request.Code);

                var exists = await repository.ExistsByCodeAsync(request.Code, ct);

                existsCheckActivity?.SetTag("tenant.exists", exists);
                existsCheckActivity?.SetStatus(ActivityStatusCode.Ok);

                activity?.SetTag("tenant.exists", exists);

                if (exists)
                {
                    errors.Add(new CreateTenantRequestValidationErrorDto(
                        Code: "TENANT_CODE_ALREADY_EXISTS",
                        Message: $"Tenant with code '{request.Code}' already exists."
                    ));
                }
            }
            catch (Exception ex)
            {
                existsCheckActivity?.SetTag("error", true);
                existsCheckActivity?.SetTag("error.type", ex.GetType().FullName);
                existsCheckActivity?.SetTag("error.message", ex.Message);
                existsCheckActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }

            if (errors.Count > 0)
            {
                activity?.SetTag("tenant.validation.result", "failed");
                activity?.SetTag("tenant.validation.errors.count", errors.Count);
                activity?.SetTag("tenant.create.result", "validation_failed");
                activity?.SetStatus(ActivityStatusCode.Ok);

                return AppResult<object>.Fail(
                    httpStatus: 409,
                    statusCode: "TENANT_VALIDATION_FAILED",
                    message: "Tenant validation failed.",
                    data: errors
                );
            }

            var tenant = new Tenant(
                code: request.Code,
                name: request.Name);

            using var createActivity = activitySource.StartActivity("tenant.create");
            try
            {
                createActivity?.SetTag("service", nameof(CreateTenantV1));
                createActivity?.SetTag("operation", "CreateAsync");
                createActivity?.SetTag("tenant.code", request.Code);

                await repository.CreateAsync(tenant, ct);
                await unitOfWork.SaveChangesAsync(ct);

                createActivity?.SetTag("tenant.id", tenant.Id);
                createActivity?.SetStatus(ActivityStatusCode.Ok);
            }
            catch (Exception ex)
            {
                createActivity?.SetTag("error", true);
                createActivity?.SetTag("error.type", ex.GetType().FullName);
                createActivity?.SetTag("error.message", ex.Message);
                createActivity?.SetStatus(ActivityStatusCode.Error, ex.Message);
                throw;
            }

            activity?.SetTag("tenant.id", tenant.Id);
            activity?.SetTag("tenant.create.result", "success");
            activity?.SetStatus(ActivityStatusCode.Ok);

            return AppResult<object>.Ok(
                httpStatus: 201,
                statusCode: "TENANT_CREATED",
                message: "Tenant created successfully.",
                data: new CreateTenantResponseDto(tenant.Id)
            );
        }
        catch (Exception ex)
        {
            activity?.SetTag("tenant.create.result", "error");
            activity?.SetTag("error", true);
            activity?.SetTag("error.type", ex.GetType().FullName);
            activity?.SetTag("error.message", ex.Message);
            activity?.SetStatus(ActivityStatusCode.Error, ex.Message);

            logger.LogError(
                ex,
                "An error occurred while creating tenant with code '{TenantCode}'.",
                request.Code);

            return AppResult<object>.Fail(
                httpStatus: 500,
                statusCode: "INTERNAL_SERVER_ERROR",
                message: "An unexpected error occurred while creating tenant."
            );
        }
    }
}
