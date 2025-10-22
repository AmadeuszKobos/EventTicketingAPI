using Domain;

namespace Repositories.Repos
{
  public interface IEventRepository : IBaseRepository<Event>
  {
    new IQueryable<Event> GetAll();
    IQueryable<Event> GetById(Guid id);
    Task AddAsync(Event entity, CancellationToken ct = default);
    Task UpdateAsync(Event entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
  }
}
