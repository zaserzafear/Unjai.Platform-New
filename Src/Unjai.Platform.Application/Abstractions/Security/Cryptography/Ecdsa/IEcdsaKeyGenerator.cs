namespace Unjai.Platform.Application.Abstractions.Security.Cryptography.Ecdsa;

public interface IEcdsaKeyGenerator
{
    (string privatePem, string publicPem, string kid) Create();
}
