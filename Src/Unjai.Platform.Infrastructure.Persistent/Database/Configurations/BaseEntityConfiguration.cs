using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unjai.Platform.Domain.Abstractions;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Configurations;

internal abstract class BaseEntityConfiguration<TEntity>
    : IEntityTypeConfiguration<TEntity>
    where TEntity : EntityBase
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        // ---------- Soft delete ----------
        builder.Property(e => e.IsDeleted)
               .HasColumnName("is_deleted")
               .HasDefaultValue(false);

        builder.Property(e => e.DeletedAt)
               .HasColumnName("deleted_at");

        // ---------- Audit ----------
        builder.Property(e => e.CreatedAt)
               .HasColumnName("created_at")
               .HasDefaultValueSql("NOW()")
               .ValueGeneratedOnAdd();

        builder.Property(e => e.UpdatedAt)
               .HasColumnName("updated_at")
               .HasDefaultValueSql("NOW()")
               .ValueGeneratedOnAddOrUpdate();

        // ---------- Base index ----------
        builder.HasIndex(e => new { e.CreatedAt, e.Id })
               .IsDescending(false, false)
               .HasFilter("\"is_deleted\" = false")
               .HasDatabaseName($"idx_{GetTableName(builder)}_active_created_at_id");
    }

    protected static string GetTableName(EntityTypeBuilder<TEntity> builder)
        => builder.Metadata.GetTableName()!;
}
