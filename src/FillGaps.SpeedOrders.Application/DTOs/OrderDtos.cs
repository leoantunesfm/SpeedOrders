namespace FillGaps.SpeedOrders.Application.DTOs;

public record CreateOrderInput(
    Guid CustomerId, 
    decimal TotalAmount
    );

public record OrderResult(
    Guid OrderId, 
    string Status, 
    DateTime CreatedAt);