namespace Unjai.Platform.Domain.Abstractions;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync(CancellationToken ct);
    void AddOutboxMessage<T>(T integrationEvent) where T : class;
}
