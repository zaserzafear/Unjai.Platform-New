using Unjai.Platform.Domain.Primitives;

namespace Unjai.Platform.Domain.Abstractions;

public abstract class EntityBase : DomainEventManager, IEntityBase
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
