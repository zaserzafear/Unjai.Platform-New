using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Repositories.Tenants;
using Unjai.Platform.Contracts.Models;
using Unjai.Platform.Contracts.Tenants.Dtos;

namespace Unjai.Platform.Application.Services.Tenants.UpdateTenant;

public interface IUpdateTenantV1
{
    public Task<AppResult<object>> Handle(Guid id, UpdateTenantRequestDto request, CancellationToken cancellationToken);
}

internal sealed class UpdateTenantV1(ILogger<UpdateTenantV1> logger, ITenantRepository repository) : IUpdateTenantV1
{
    public async Task<AppResult<object>> Handle(Guid id, UpdateTenantRequestDto request, CancellationToken cancellationToken)
    {
        try
        {
            var errors = new List<CreateTenantRequestValidationErrorDto>();

            var nameExists = await repository.ExistsByNameAsync(request.Name, cancellationToken);
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

            var entity = await repository.GetByIdAsync(id, cancellationToken);
            if (entity is null)
            {
                return AppResult<object>.Fail(
                    httpStatus: 404,
                    statusCode: "TENANT_NOT_FOUND",
                    message: "Tenant not found."
                );
            }

            entity.Name = request.Name;

            await repository.UpdateAsync(entity, cancellationToken);

            return AppResult<object>.Ok(
                httpStatus: 200,
                statusCode: "TENANT_UPDATED",
                message: "Tenant updated successfully."
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while updating tenant with id '{TenantId}'",
                id);

            return AppResult<object>.Fail(
                httpStatus: 500,
                statusCode: "INTERNAL_SERVER_ERROR",
                message: "An unexpected error occurred while updating tenant."
            );
        }
    }
}
