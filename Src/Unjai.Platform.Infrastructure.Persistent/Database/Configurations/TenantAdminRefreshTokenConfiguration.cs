using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unjai.Platform.Domain.Entities.TenantsAdminRefreshToken;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Configurations;

internal sealed class TenantAdminRefreshTokenConfiguration
    : IEntityTypeConfiguration<TenantAdminRefreshToken>
{
    public void Configure(EntityTypeBuilder<TenantAdminRefreshToken> entity)
    {
        entity.ToTable("tenant_admin_refresh_tokens");

        entity.HasKey(e => e.Id);

        entity.Property(e => e.Id)
               .HasColumnName("id")
               .HasDefaultValueSql("uuidv7()");

        entity.Property(e => e.TenantAdminId)
              .HasColumnName("tenant_admin_id")
              .IsRequired();

        entity.Property(e => e.Token)
              .HasColumnName("token")
              .HasMaxLength(500)
              .IsRequired();

        entity.Property(e => e.ExpiresAt)
              .HasColumnName("expires_at")
              .IsRequired();

        entity.Property(e => e.IsRevoked)
              .HasColumnName("is_revoked")
              .IsRequired()
              .HasDefaultValue(false);

        entity.Property(e => e.CreatedAt)
              .HasColumnName("created_at")
              .IsRequired()
              .HasDefaultValueSql("now()")
              .ValueGeneratedOnAdd();

        entity.HasIndex(e => e.Token)
              .IsUnique();
    }
}
