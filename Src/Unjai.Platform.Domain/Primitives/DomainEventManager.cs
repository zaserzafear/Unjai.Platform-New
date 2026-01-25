namespace Unjai.Platform.Domain.Primitives;

public abstract class DomainEventManager
{
    private readonly List<IDomainEvent> _events = new();

    protected DomainEventManager() { }

    public void RaiseEvent(IDomainEvent domainEvent)
    {
        _events.Add(domainEvent);
    }

    public IEnumerable<IDomainEvent> GetEvents()
    {
        return _events.ToList();
    }

    public void ClearEvents()
    {
        _events.Clear();
    }
}
