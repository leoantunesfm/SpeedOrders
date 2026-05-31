using FillGaps.SpeedOrders.Domain.Entities;

namespace FillGaps.SpeedOrders.Application.DTOs;

public record OrderSummaryDto
{
    public Guid OrderId { get; init; }
    public Guid CustomerId { get; init; }
    public decimal TotalAmount { get; init; }
    public OrderStatus Status { get; init; } 
    public DateTime CreatedAt { get; init; }
    public string StatusDescription => Status.ToString(); 
}