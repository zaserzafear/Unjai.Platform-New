using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unjai.Platform.Domain.Entities.TenantsAdminPermission;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Configurations;

internal sealed class TenantAdminPermissionConfiguration
    : IEntityTypeConfiguration<TenantAdminPermission>
{
    public void Configure(EntityTypeBuilder<TenantAdminPermission> entity)
    {
        entity.ToTable("tenants_admin_permissions");

        entity.HasKey(x => x.Id);

        entity.Property(x => x.Id)
            .HasColumnName("id");

        entity.Property(x => x.Code)
            .HasColumnName("code")
            .IsRequired()
            .HasMaxLength(100);

        entity.Property(x => x.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasMaxLength(150);

        entity.HasIndex(x => x.Code)
            .IsUnique();

        entity.HasData(TenantAdminPermissionSeed.GetSeedData());
    }
}
