using FillGaps.SpeedOrders.Domain.Entities;
using FillGaps.SpeedOrders.Domain.Interfaces;
using FillGaps.SpeedOrders.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FillGaps.SpeedOrders.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : Entity
{
    protected readonly SpeedOrdersDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(SpeedOrdersDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
    }

    public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
}