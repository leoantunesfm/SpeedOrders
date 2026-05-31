using FillGaps.SpeedOrders.Application.DTOs;
using FillGaps.SpeedOrders.Application.Events;
using FillGaps.SpeedOrders.Application.Interfaces;
using FillGaps.SpeedOrders.Domain.Entities;
using FillGaps.SpeedOrders.Domain.Interfaces;

namespace FillGaps.SpeedOrders.Application.Services;

public class OrderAppService(
    IOrderRepository orderRepository,
    IUnitOfWork unitOfWork,
    IMessagePublisher messagePublisher) : IOrderAppService
{
    public async Task<OrderResult> CreateOrderAsync(CreateOrderInput input, CancellationToken cancellationToken = default)
    {
        var order = new Order(input.CustomerId, input.TotalAmount);

        await orderRepository.AddAsync(order, cancellationToken);

        var committed = await unitOfWork.CommitAsync(cancellationToken);

        if (!committed)
        {
            throw new ApplicationException("Falha ao salvar o pedido no banco de dados.");
        }

        var orderCreatedEvent = new OrderCreatedEvent(
            order.Id,
            order.CustomerId,
            order.TotalAmount,
            DateTime.UtcNow
        );

        await messagePublisher.PublishAsync("order-created-topic", orderCreatedEvent, cancellationToken);

        return new OrderResult(order.Id, order.Status.ToString(), order.CreatedAt);
    }
}