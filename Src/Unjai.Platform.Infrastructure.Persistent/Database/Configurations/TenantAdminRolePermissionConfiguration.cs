using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unjai.Platform.Domain.Entities.TenantsAdminRolePermission;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Configurations;

internal sealed class TenantAdminRolePermissionConfiguration
     : IEntityTypeConfiguration<TenantAdminRolePermission>
{
    public void Configure(EntityTypeBuilder<TenantAdminRolePermission> entity)
    {
        entity.ToTable("tenants_admin_role_permissions");

        entity.Property(x => x.RoleId)
            .HasColumnName("role_id");

        entity.Property(x => x.PermissionId)
            .HasColumnName("permission_id");

        entity.HasKey(x => new { x.RoleId, x.PermissionId });

        entity.HasOne(x => x.Role)
            .WithMany(x => x.RolePermissions)
            .HasForeignKey(x => x.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        entity.HasOne(x => x.Permission)
            .WithMany()
            .HasForeignKey(x => x.PermissionId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
