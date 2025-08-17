using System.ComponentModel.DataAnnotations.Schema;

namespace Dzeta.Extensions.CQRS;

public abstract class BaseEntity
{
    [NotMapped] readonly List<IDomainEvent> domainEvents = new();

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    [NotMapped] public IReadOnlyCollection<IDomainEvent> DomainEvents => domainEvents.AsReadOnly();

    public void AddDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Add(domainEvent);
    }

    public void RemoveDomainEvent(IDomainEvent domainEvent)
    {
        domainEvents.Remove(domainEvent);
    }

    public void ClearDomainEvents()
    {
        domainEvents.Clear();
    }
}