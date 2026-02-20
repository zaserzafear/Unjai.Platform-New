namespace Unjai.Platform.Application.Services.JwtKeyStores;

internal static class JwtKeyStoreCacheKeys
{
    public static string GetAllPublicKeys => "JwtKeyStore:GetAllPublicKeys";
    public static string GetByKid(string kid) => $"JwtKeyStore:GetByKid:{kid}";
}
