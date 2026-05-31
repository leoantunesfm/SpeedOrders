namespace FillGaps.SpeedOrders.Application.Events;

public record OrderCreatedEvent(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    DateTime OccurredOn
);