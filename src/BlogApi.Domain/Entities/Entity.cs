using BlogApi.Domain.Interfaces;

namespace BlogApi.Domain.Entities;

public class Entity
{
    public int Id { get; set; }
    private readonly List<IDomainEvent> _domainEvents = [];
    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents;

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void Raise(IDomainEvent domainEvent)
       => _domainEvents.Add(domainEvent);
}
