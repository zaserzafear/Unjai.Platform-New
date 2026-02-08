namespace Unjai.Platform.Domain.Abstractions;

public interface ITenantOwned
{
    Guid TenantId { get; set; }
}
