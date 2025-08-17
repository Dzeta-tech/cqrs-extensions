## Dzeta.Extensions.CQRS

Utilities for connecting CQRS with EF Core and MediatR.

### Features
- **BaseEntity**: domain events collection and `UpdatedAt` maintenance.
- **IDomainEvent**: marker interface extending MediatR `INotification`.
- **IUnitOfWork**: abstraction for committing changes (`SaveChangesAsync`).
- **CqrsDbContext**: abstract context that dispatches domain events after persistence, with cycle protection and a virtual `MaxEventDispatchIterations`.

### Install
Add the package from NuGet (after publishing):

```bash
dotnet add package Dzeta.Extensions.CQRS
```

### Usage
1) Inherit your entities from `Dzeta.Extensions.CQRS.BaseEntity` and raise events:

```csharp
public sealed class Wallet : BaseEntity
{
    public Guid Id { get; private set; }
    public decimal Balance { get; private set; }

    public void Deposit(decimal amount)
    {
        Balance += amount;
        AddDomainEvent(new DepositHappened(Id, amount));
    }
}
```

2) Implement events as `IDomainEvent` and MediatR handlers in your app layer.

3) Derive your EF Core context from `CqrsDbContext<TContext>` and call `SaveChangesAsync` as usual. Events will be dispatched via `IPublisher` after each successful save until the queue is empty. Override `MaxEventDispatchIterations` if needed.

### Packaging
This project is configured to generate a NuGet package on build. Run:

```bash
dotnet build -c Release
```

Or to pack explicitly:

```bash
dotnet pack -c Release
```

### License
MIT


