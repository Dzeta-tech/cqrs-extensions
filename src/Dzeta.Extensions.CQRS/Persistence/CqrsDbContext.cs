using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Dzeta.Extensions.CQRS;

public abstract class CqrsDbContext<TContext>(DbContextOptions<TContext> options, IPublisher publisher)
    : DbContext(options) where TContext : DbContext
{
    protected virtual IPublisher Publisher { get; } = publisher;

    protected virtual int MaxEventDispatchIterations => 10;

    public override async Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        int total = 0;
        int iterations = 0;

        while (true)
        {
            iterations++;
            if (iterations > MaxEventDispatchIterations)
                throw new InvalidOperationException(
                    $"Domain event dispatch loop exceeded {MaxEventDispatchIterations} iterations (possible cycle).");

            List<IDomainEvent> events = DequeueAndClearDomainEvents();

            if (ChangeTracker.HasChanges())
                total += await base.SaveChangesAsync(ct);

            if (events.Count == 0)
                break;

            foreach (IDomainEvent domainEvent in events)
                await Publisher.Publish(domainEvent, ct);
        }

        return total;
    }

    protected virtual List<IDomainEvent> DequeueAndClearDomainEvents()
    {
        List<BaseEntity> entitiesWithEvents = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.Entity.DomainEvents.Count > 0)
            .Select(e => e.Entity)
            .ToList();

        List<IDomainEvent> events = entitiesWithEvents
            .SelectMany(e => e.DomainEvents)
            .ToList();

        foreach (BaseEntity entity in entitiesWithEvents)
        {
            entity.ClearDomainEvents();
            entity.UpdatedAt = DateTime.UtcNow;
        }

        return events;
    }
}