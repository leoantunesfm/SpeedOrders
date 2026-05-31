using FillGaps.SpeedOrders.Application.DTOs;

namespace FillGaps.SpeedOrders.Application.Interfaces;

public interface IOrderQueries
{
    Task<IEnumerable<OrderSummaryDto>> GetOrdersByCustomerAsync(Guid customerId, CancellationToken cancellationToken = default);
}