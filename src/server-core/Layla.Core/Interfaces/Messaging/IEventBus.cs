namespace Layla.Core.Interfaces.Messaging;

public interface IEventBus
{
    void Publish<T>(T @event, string exchangeName, string routingKey = "") where T : class;
}
