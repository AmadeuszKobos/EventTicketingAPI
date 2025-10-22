using Domain;

namespace Repositories.Repos
{
  public interface ITicketRepository : IBaseRepository<Ticket>
  {
    new IQueryable<Ticket> GetAll();

    IQueryable<Ticket> GetById(Guid id);

    IQueryable<Ticket> GetByIdsWithPrice(IEnumerable<Guid> ids); // used by OrderService

    Task AddAsync(Ticket entity, CancellationToken ct = default);

    Task UpdateAsync(Ticket entity, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);

    Task MarkSoldAsync(IEnumerable<Ticket> tickets, CancellationToken ct = default); // used by OrderService
  }
}