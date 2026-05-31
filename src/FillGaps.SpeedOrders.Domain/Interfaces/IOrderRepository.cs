using FillGaps.SpeedOrders.Domain.Entities;

namespace FillGaps.SpeedOrders.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    // Aqui no futuro você coloca métodos ESPECÍFICOS de pedidos.
    // Exemplo: Task<IEnumerable<Order>> GetPendingOrdersAsync();
}