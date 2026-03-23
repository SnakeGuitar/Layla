namespace Layla.Core.Interfaces.Messaging;

public interface IEventPublisher
{
    /// <summary>
    /// Publishes an event asynchronously.
    /// Returns <c>true</c> if the event was published successfully; <c>false</c> if the broker
    /// is unavailable or the publish operation failed.
    /// </summary>
    Task<bool> PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class;
}
