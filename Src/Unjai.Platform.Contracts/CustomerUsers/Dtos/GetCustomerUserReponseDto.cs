namespace Unjai.Platform.Contracts.CustomerUsers.Dtos;

public sealed record GetCustomerUserReponseDto(Guid Id, string FirstName, string LastName, string Email, bool EmailVerified);
