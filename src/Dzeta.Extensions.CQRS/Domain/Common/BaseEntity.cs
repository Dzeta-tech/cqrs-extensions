using System.ComponentModel.DataAnnotations.Schema;

namespace Dzeta.Extensions.CQRS;

public abstract class BaseEntity
{
	[NotMapped]
	private readonly List<IDomainEvent> _domainEvents = new();

	public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

	[NotMapped]
	public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

	public void AddDomainEvent(IDomainEvent domainEvent)
	{
		_domainEvents.Add(domainEvent);
	}

	public void RemoveDomainEvent(IDomainEvent domainEvent)
	{
		_domainEvents.Remove(domainEvent);
	}

	public void ClearDomainEvents()
	{
		_domainEvents.Clear();
	}
}


