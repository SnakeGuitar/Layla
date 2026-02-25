using Layla.Core.Interfaces.Messaging;
using Microsoft.Extensions.Logging;

namespace Layla.Infrastructure.Messaging;

public class DummyEventPublisher : IEventPublisher
{
    private readonly ILogger<DummyEventPublisher> _logger;

    public DummyEventPublisher(ILogger<DummyEventPublisher> logger)
    {
        _logger = logger;
    }

    public Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default) where TEvent : class
    {
        _logger.LogInformation("Dummy PublishAsync: Event {EventType} published.", typeof(TEvent).Name);
        return Task.CompletedTask;
    }
}
