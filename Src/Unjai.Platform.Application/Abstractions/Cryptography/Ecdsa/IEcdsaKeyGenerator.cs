namespace Unjai.Platform.Application.Abstractions.Cryptography.Ecdsa;

public interface IEcdsaKeyGenerator
{
    (string privatePem, string publicPem, string kid) Create();
}
