using Domain;

namespace Repositories.Repos
{
  public interface IOrderRepository : IBaseRepository<Order>
  {
    new IQueryable<Order> GetAll();
    IQueryable<Order> GetById(Guid id);
    Task AddAsync(Order entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
  }
}
