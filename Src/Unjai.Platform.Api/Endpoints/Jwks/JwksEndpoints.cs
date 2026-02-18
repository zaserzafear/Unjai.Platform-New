using Unjai.Platform.Api.Endpoints.Extensions;
using Unjai.Platform.Infrastructure.Security.Authentication.Jwt;

namespace Unjai.Platform.Api.Endpoints.Jwks;

public sealed class JwksEndpoints : IEndpoint
{
    private const string Base = "jwks";
    private const string Tag = "Jwks";

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup(Base)
            .WithTags(Tag);

        group.MapPost("rotate", async (
            IJwtKeyStore keyStore,
            CancellationToken ct) =>
        {
            keyStore.RotateKey();
            return Results.NoContent();
        });
    }
}
