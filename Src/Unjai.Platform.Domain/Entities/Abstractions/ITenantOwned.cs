namespace Unjai.Platform.Domain.Entities.Abstractions;

public interface ITenantOwned
{
    Guid TenantId { get; set; }
}
