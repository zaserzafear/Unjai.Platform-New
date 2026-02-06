using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Unjai.Platform.Domain.Entities.OutboxMessages;

namespace Unjai.Platform.Infrastructure.Persistent.Database.Configurations.OutboxMessages;

internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> entity)
    {
        entity.ToTable("outbox_messages");

        entity.HasKey(o => o.Id);

        entity.Property(o => o.Id)
              .HasColumnName("id")
              .HasDefaultValueSql("uuidv7()");

        entity.Property(o => o.Type)
              .HasColumnName("type")
              .HasMaxLength(255)
              .IsRequired();

        entity.Property(o => o.Content)
              .HasColumnName("content")
              .HasColumnType("jsonb")
              .IsRequired();

        entity.Property(o => o.OccurredOnUtc)
              .HasColumnName("occurred_on_utc")
              .HasColumnType("timestamp with time zone")
              .IsRequired();

        entity.Property(o => o.ProcessedOnUtc)
              .HasColumnName("processed_on_utc")
              .HasColumnType("timestamp with time zone");

        entity.Property(o => o.Error)
              .HasColumnName("error")
              .HasColumnType("text");

        entity.HasIndex(o => new { o.OccurredOnUtc, o.ProcessedOnUtc })
               .HasDatabaseName("idx_outbox_messages_unprocessed")
               .IncludeProperties(o => new { o.Id, o.Type, o.Content })
               .HasFilter("\"processed_on_utc\" IS NULL");
    }
}
