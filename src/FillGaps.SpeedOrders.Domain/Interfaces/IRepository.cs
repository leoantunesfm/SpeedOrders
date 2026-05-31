using FillGaps.SpeedOrders.Domain.Entities;

namespace FillGaps.SpeedOrders.Domain.Interfaces;

public interface IRepository<T> where T : Entity 
{
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}