namespace Layla.Core.Interfaces.Messaging;

public interface IEventPublisher
{
    Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class;
}
