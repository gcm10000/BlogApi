using MediatR;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using BlogApi.Domain.Entities;

namespace BlogApi.Application.Infrastructure.Data.Interceptors;

public sealed class PublishDomainEventsInterceptor
    : SaveChangesInterceptor, ISaveChangesInterceptor
{
    private readonly IPublisher _publisher;

    public PublishDomainEventsInterceptor(IPublisher publisher)
    {
        _publisher = publisher;
    }

    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result)
    {
        if (eventData.Context is not null)
            PublishDomainEventsAsync(eventData.Context).GetAwaiter().GetResult();

        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = default)
    {
        if (eventData.Context is not null)
            await PublishDomainEventsAsync(eventData.Context);

        var savedChangesAsresult = await base.SavedChangesAsync(eventData, result, cancellationToken);

        return savedChangesAsresult;
    }

    private async Task PublishDomainEventsAsync(DbContext context)
    {
        var domainEvents = context.ChangeTracker.Entries<Entity>()
            .Select(e => e.Entity)
            .Where(e => e.DomainEvents.Any())
            .SelectMany(e =>
            {
                var domainEvents = e.DomainEvents.ToList();

                e.ClearDomainEvents();

                return domainEvents;
            })
            .ToList();

        foreach (var domainEvent in domainEvents)
        {
            await _publisher.Publish(domainEvent);
        }
    }
}
