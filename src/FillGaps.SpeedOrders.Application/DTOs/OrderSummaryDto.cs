namespace FillGaps.SpeedOrders.Application.DTOs;

public record OrderSummaryDto(
    Guid OrderId,
    Guid CustomerId,
    decimal TotalAmount,
    string Status,
    DateTime CreatedAt
);