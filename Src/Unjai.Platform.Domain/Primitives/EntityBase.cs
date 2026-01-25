namespace Unjai.Platform.Domain.Primitives;

public abstract class EntityBase : DomainEventManager, IEntityBase
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
