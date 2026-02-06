using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unjai.Platform.Domain.Entities.Tenants;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Configurations.Tenants;

internal sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
{
    public void Configure(EntityTypeBuilder<Tenant> entity)
    {
        entity.ToTable("tenants");

        entity.HasKey(o => o.Id);

        entity.Property(o => o.Id)
            .HasColumnName("id")
            .HasDefaultValueSql("uuidv7()");

        entity.Property(o => o.Code)
            .HasColumnName("code")
            .IsRequired()
            .HasMaxLength(50);

        entity.HasIndex(o => o.Code)
            .IsUnique();

        entity.Property(o => o.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(200);

        entity.HasIndex(o => o.Name)
            .IsUnique();

        entity.Property(o => o.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        entity.Property(o => o.IsDeleted)
            .HasColumnName("is_deleted")
            .IsRequired()
            .HasDefaultValue(false);

        entity.Property(o => o.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        entity.Property(o => o.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();
    }
}
