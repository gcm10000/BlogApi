using MediatR;

namespace BlogApi.Domain.Interfaces;

public interface IDomainEvent : INotification
{
    string EventIdentifier { get; }
}
