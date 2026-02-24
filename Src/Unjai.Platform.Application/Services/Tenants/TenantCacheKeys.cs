namespace Unjai.Platform.Application.Services.Tenants;

internal static class TenantCacheKeys
{
    public static string GetById(Guid tenantId)
        => $"tenant:id:{tenantId}";
}
