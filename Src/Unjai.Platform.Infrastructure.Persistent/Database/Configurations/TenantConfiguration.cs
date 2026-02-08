using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unjai.Platform.Domain.Entities.Tenants;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Configurations;

internal sealed class TenantConfiguration : BaseEntityConfiguration<Tenant>
{
    public override void Configure(EntityTypeBuilder<Tenant> entity)
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

        base.Configure(entity);
    }
}
