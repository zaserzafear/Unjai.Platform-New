using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unjai.Platform.Domain.Entities.TenantsAdminRole;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Configurations;

internal sealed class TenantAdminRoleConfiguration
    : IEntityTypeConfiguration<TenantAdminRole>
{
    public void Configure(EntityTypeBuilder<TenantAdminRole> entity)
    {
        entity.ToTable("tenants_admin_roles");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .HasColumnName("id");

        entity.Property(x => x.Code)
            .HasColumnName("code")
            .IsRequired()
            .HasMaxLength(50);

        entity.HasIndex(x => x.Code)
            .IsUnique();

        entity.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(100);
    }
}
