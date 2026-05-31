namespace FillGaps.SpeedOrders.Application.Interfaces;

public interface IMessagePublisher
{
    Task PublishAsync<TEvent>(string topic, TEvent message, CancellationToken cancellationToken = default);
}