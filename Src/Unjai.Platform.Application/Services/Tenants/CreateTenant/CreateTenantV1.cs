using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.Tenants.Dtos;
using Unjai.Platform.Domain.Abstractions;
using Unjai.Platform.Domain.Entities.Tenants;

namespace Unjai.Platform.Application.Services.Tenants.CreateTenant;

public interface ICreateTenantV1
{
    public Task<AppResult<object>> Handle(CreateTenantRequestDto request, CancellationToken ct);
}

internal sealed class CreateTenantV1(
    ILogger<CreateTenantV1> logger,
    IUnitOfWork unitOfWork,
    ITenantRepository repository) : ICreateTenantV1
{
    public async Task<AppResult<object>> Handle(CreateTenantRequestDto request, CancellationToken ct)
    {
        try
        {
            var errors = new List<CreateTenantRequestValidationErrorDto>();

            if (await repository.ExistsByCodeAsync(request.Code, ct))
            {
                errors.Add(new(
                    Code: "TENANT_CODE_ALREADY_EXISTS",
                    Message: $"Tenant with code '{request.Code}' already exists."
                ));
            }

            if (errors.Count > 0)
            {
                return AppResult<object>.Fail(
                    httpStatus: 409,
                    statusCode: "TENANT_VALIDATION_FAILED",
                    message: "Tenant validation failed.",
                    data: errors
                );
            }

            var tenant = new Tenant
            {
                Code = request.Code,
                Name = request.Name
            };

            await repository.CreateAsync(tenant, ct);
            await unitOfWork.SaveChangesAsync(ct);

            return AppResult<object>.Ok(
                httpStatus: 201,
                statusCode: "TENANT_CREATED",
                message: "Tenant created successfully."
            );
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while creating tenant with code '{TenantCode}' and name '{TenantName}'", request.Code, request.Name);

            return AppResult<object>.Fail(
                httpStatus: 500,
                statusCode: "INTERNAL_SERVER_ERROR",
                message: ex.Message
            );
        }
    }
}
