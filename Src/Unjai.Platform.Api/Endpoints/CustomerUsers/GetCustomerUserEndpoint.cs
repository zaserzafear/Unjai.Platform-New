//using Unjai.Platform.Api.Endpoints.Extensions;
//using Unjai.Platform.Api.Models;
//using Unjai.Platform.Application.Services.CustomerUsers.GetCustomerUser;
//using Unjai.Platform.Infrastructure.RateLimiting.Core;
//using Unjai.Platform.Infrastructure.RateLimiting.Extensions;

//namespace Unjai.Platform.Api.Endpoints.CustomerUsers;

//public sealed class GetUserEndpoint : IEndpoint
//{
//    public void MapEndpoint(IEndpointRouteBuilder app)
//    {
//        var group = app
//            .MapGroup(CustomerUserEndpoints.Base)
//            .WithTags(CustomerUserEndpoints.Tag);

//        group.MapGet("{id:guid}", async (Guid id, IGetCustomerUserV1 useCase, CancellationToken cancellationToken = default) =>
//        {
//            var result = await useCase.Handle(id, cancellationToken);

//            return ApiResponseResults.ToHttpResult(result);
//        })
//            .EnforceRateLimit(RateLimitPolicyKeys.GetUser)
//            .MapToApiVersion(1);
//    }
//}
