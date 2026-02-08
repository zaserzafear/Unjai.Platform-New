using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Unjai.Platform.Domain.Abstractions;
using Unjai.Platform.Infrastructure.Persistent.Database;
using Unjai.Platform.Infrastructure.Persistent.Outbox;

namespace Unjai.Platform.Infrastructure.Persistent.Repositories;

internal sealed class UnitOfWork(WriteDbContext writeDbContext) : IUnitOfWork
{
    private readonly List<OutboxMessage> outboxMessages = new();

    public void AddOutboxMessage<T>(T integrationEvent) where T : class
    {
        ArgumentNullException.ThrowIfNull(integrationEvent);

        var outboxMessage = new OutboxMessage
        {
            Id = Guid.NewGuid(),
            Type = integrationEvent.GetType().FullName!,
            Content = JsonSerializer.Serialize(integrationEvent, integrationEvent.GetType()),
            OccurredOnUtc = DateTime.UtcNow
        };

        outboxMessages.Add(outboxMessage);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct)
    {
        UpdateBaseEntityFields();

        if (outboxMessages.Count > 0)
        {
            writeDbContext.Set<OutboxMessage>().AddRange(outboxMessages);
        }

        await using var transaction =
            await writeDbContext.Database.BeginTransactionAsync(ct);

        var rowsAffected = await writeDbContext.SaveChangesAsync(ct);

        await transaction.CommitAsync(ct);

        outboxMessages.Clear();

        return rowsAffected;
    }

    private void UpdateBaseEntityFields()
    {
        var utcNow = DateTime.UtcNow;

        foreach (var entry in writeDbContext.ChangeTracker.Entries<EntityBase>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.UpdatedAt = utcNow;
                    entry.Entity.IsDeleted = false;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;
                    break;

                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.UpdatedAt = utcNow;
                    entry.Entity.DeletedAt = utcNow;
                    break;
            }
        }
    }
}
