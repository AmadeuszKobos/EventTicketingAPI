using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repos
{
  public class OrderRepository : BaseRepository<Order>, IOrderRepository
  {
    public OrderRepository(AppDbContext context) : base(context) { }

    public new IQueryable<Order> GetAll() =>
        _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.OrderTickets).ThenInclude(ot => ot.Ticket).ThenInclude(t => t.PriceTier);

    public IQueryable<Order> GetById(Guid id) =>
        _context.Orders
            .AsNoTracking()
            .Include(o => o.Customer)
            .Include(o => o.OrderTickets).ThenInclude(ot => ot.Ticket).ThenInclude(t => t.PriceTier)
            .Where(o => o.OrderId == id);

    public async Task AddAsync(Order entity, CancellationToken ct = default)
    {
      _context.Orders.Add(entity);
      await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
      var e = await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == id, ct);
      if (e is null) return;
      _context.Orders.Remove(e);
      await _context.SaveChangesAsync(ct);
    }
  }
}
