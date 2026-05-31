using FillGaps.SpeedOrders.Domain.Interfaces;

namespace FillGaps.SpeedOrders.Infrastructure.Data;

public class UnitOfWork : IUnitOfWork
{
    private readonly SpeedOrdersDbContext _context;

    public UnitOfWork(SpeedOrdersDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CommitAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken) > 0;
    }
}