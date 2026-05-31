using FillGaps.SpeedOrders.Application.DTOs;

namespace FillGaps.SpeedOrders.Application.Interfaces;

public interface IOrderAppService
{
    Task<OrderResult> CreateOrderAsync(CreateOrderInput input, CancellationToken cancellationToken = default);
}