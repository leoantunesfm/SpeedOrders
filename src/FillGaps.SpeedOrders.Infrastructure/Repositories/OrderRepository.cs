using FillGaps.SpeedOrders.Domain.Entities;
using FillGaps.SpeedOrders.Domain.Interfaces;
using FillGaps.SpeedOrders.Infrastructure.Data;

namespace FillGaps.SpeedOrders.Infrastructure.Repositories;

public class OrderRepository : Repository<Order>, IOrderRepository
{
    public OrderRepository(SpeedOrdersDbContext context) : base(context)
    {
    }
}