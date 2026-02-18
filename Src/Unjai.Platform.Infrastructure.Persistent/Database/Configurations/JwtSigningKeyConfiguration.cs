using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unjai.Platform.Domain.Entities.JwtSigningKeys;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Configurations;

internal sealed class JwtSigningKeyConfiguration
     : IEntityTypeConfiguration<JwtSigningKey>
{
    public void Configure(EntityTypeBuilder<JwtSigningKey> entity)
    {
        entity.ToTable("jwt_signing_key");

        entity.HasKey(o => o.KeyId);

        entity.Property(o => o.KeyId)
              .HasColumnName("key_id");

        entity.Property(o => o.PublicKeyPem)
              .HasColumnName("public_key_pem")
              .HasColumnType("text");

        entity.Property(o => o.PrivateKeyPem)
              .HasColumnName("private_key_pem")
              .HasColumnType("text");

        entity.Property(o => o.IsActive)
              .HasColumnName("is_active")
              .HasDefaultValue(true);

        entity.HasIndex(o => o.IsActive)
              .IsUnique()
              .HasFilter("\"is_active\" = true")
              .HasDatabaseName("idx_jwt_signing_key_active");

        entity.Property(e => e.CreatedAt)
              .HasColumnName("created_at")
              .HasDefaultValueSql("NOW()")
              .ValueGeneratedOnAdd();
    }
}
