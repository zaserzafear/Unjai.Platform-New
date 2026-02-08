using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.Tenants.Dtos;
using Unjai.Platform.Domain.Entities.Tenants;

namespace Unjai.Platform.Application.Services.Tenants.CreateTenant;

public interface ICreateTenantV1
{
    public Task<AppResult<object>> Handle(CreateTenantRequestDto request, CancellationToken cancellationToken);
}

internal sealed class CreateTenantV1(ILogger<CreateTenantV1> logger, ITenantRepository repository) : ICreateTenantV1
{
    public async Task<AppResult<object>> Handle(CreateTenantRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var codeExists = await repository.ExistsByCodeAsync(request.Code, cancellationToken);
            var nameExists = await repository.ExistsByNameAsync(request.Name, cancellationToken);

            var errors = new List<CreateTenantRequestValidationErrorDto>();

            if (codeExists)
            {
                errors.Add(new CreateTenantRequestValidationErrorDto(
                    Code: "TENANT_CODE_ALREADY_EXISTS",
                    Message: $"Tenant with code '{request.Code}' already exists."
                ));
            }

            if (nameExists)
            {
                errors.Add(new CreateTenantRequestValidationErrorDto(
                    Code: "TENANT_NAME_ALREADY_EXISTS",
                    Message: $"Tenant with name '{request.Name}' already exists."
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

            var entity = new Tenant
            {
                Code = request.Code,
                Name = request.Name,
            };

            var result = await repository.CreateAsync(entity, cancellationToken);
            if (result is not null)
            {
                return AppResult<object>.Ok(
                    httpStatus: 201,
                    statusCode: "TENANT_CREATED",
                    message: "Tenant created successfully."
                );
            }
            else
            {
                return AppResult<object>.Fail(
                    httpStatus: 500,
                    statusCode: "TENANT_CREATION_FAILED",
                    message: "Failed to create tenant."
                );
            }
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
