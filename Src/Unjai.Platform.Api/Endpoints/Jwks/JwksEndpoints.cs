using Unjai.Platform.Api.Endpoints.Extensions;
using Unjai.Platform.Application.Services.JwtKeyStores;

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
            IJwtKeyStoreService keyStore,
            CancellationToken ct) =>
        {
            await keyStore.RotateKeyAsync(TimeSpan.FromDays(7), ct);
            return Results.NoContent();
        });
    }
}
