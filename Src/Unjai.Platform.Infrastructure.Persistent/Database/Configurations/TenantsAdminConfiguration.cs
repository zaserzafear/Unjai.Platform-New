using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unjai.Platform.Domain.Entities.TenantsAdmin;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Configurations;

internal sealed class TenantsAdminConfiguration
    : BaseEntityConfiguration<TenantAdmin>
{
    public override void Configure(EntityTypeBuilder<TenantAdmin> entity)
    {
        entity.ToTable("tenants_admin");

        entity.Property(x => x.Username)
            .HasColumnName("username")
            .IsRequired()
            .HasMaxLength(100);

        entity.HasIndex(x => x.Username)
            .IsUnique();

        entity.Property(x => x.PasswordHash)
            .HasColumnName("password_hash")
            .IsRequired();

        entity.Property(x => x.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        entity.Property(x => x.RoleId)
            .HasColumnName("role_id")
            .IsRequired();

        base.Configure(entity);
    }
}
