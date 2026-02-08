using Microsoft.Extensions.Logging;
using Unjai.Platform.Application.Services.CustomerUsers.Exceptions;
using Unjai.Platform.Contracts.CustomerUsers.Dtos;
using Unjai.Platform.Contracts.Models;

namespace Unjai.Platform.Application.Services.CustomerUsers.GetCustomerUser;

public interface IGetCustomerUserV1
{
    Task<AppResult<GetCustomerUserReponseDto>> Handle(Guid userId, CancellationToken ct);
}

internal sealed class GetCustomerUserV1(ILogger<GetCustomerUserV1> logger) : IGetCustomerUserV1
{
    public async Task<AppResult<GetCustomerUserReponseDto>> Handle(Guid userId, CancellationToken ct)
    {
        try
        {
            throw new CustomerUserNotFoundException($"User Id {userId} Not Found.");
        }
        catch (CustomerUserNotFoundException ex)
        {
            logger.LogWarning(
                ex,
                "Customer user not found. UserId: {UserId}",
                userId
            );

            return AppResult<GetCustomerUserReponseDto>.Fail(
                httpStatus: 404,
                statusCode: "CUSTOMER_USER_NOT_FOUND",
                message: ex.Message
            );
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to get customer user due to an unexpected error. UserId: {UserId}",
                userId
            );

            return AppResult<GetCustomerUserReponseDto>.Fail(
                httpStatus: 500,
                statusCode: "INTERNAL_SERVER_ERROR",
                message: ex.Message
            );
        }
    }
}
