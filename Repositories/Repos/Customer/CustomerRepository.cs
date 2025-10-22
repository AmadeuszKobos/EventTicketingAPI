using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repos
{
  public class CustomerRepository : BaseRepository<Customer>, ICustomerRepository
  {
    public CustomerRepository(AppDbContext context) : base(context)
    {
    }

    public new IQueryable<Customer> GetAll()
    {
      return _context.Customers
          .AsNoTracking()
          .Include(c => c.Orders)
              .ThenInclude(o => o.OrderTickets)
                  .ThenInclude(ot => ot.Ticket);
    }

    public IQueryable<Customer> GetById(Guid id)
    {
      return _context.Customers
          .AsNoTracking()
          .Include(c => c.Orders)
              .ThenInclude(o => o.OrderTickets)
                  .ThenInclude(ot => ot.Ticket)
          .Where(c => c.CustomerId == id);
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default) =>
    await _context.Customers.AnyAsync(c => c.CustomerId == id, ct);
  }
}
