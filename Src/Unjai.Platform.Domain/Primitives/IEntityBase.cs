namespace Unjai.Platform.Domain.Primitives;

public interface IEntityBase
{
    DateTime CreatedAt { get; set; }
    DateTime UpdatedAt { get; set; }
    bool IsDeleted { get; set; }
    DateTime? DeletedAt { get; set; }
}
