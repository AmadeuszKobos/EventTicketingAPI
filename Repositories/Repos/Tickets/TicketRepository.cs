using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repos
{
  public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
  {
    public TicketRepository(AppDbContext context) : base(context)
    {
    }

    public new IQueryable<Ticket> GetAll() =>
        _context.Tickets.AsNoTracking()
            .Include(t => t.Event)
            .Include(t => t.PriceTier);

    public IQueryable<Ticket> GetById(Guid id) =>
        _context.Tickets.AsNoTracking()
            .Include(t => t.Event)
            .Include(t => t.PriceTier)
            .Where(t => t.TicketId == id);

    public IQueryable<Ticket> GetByIdsWithPrice(IEnumerable<Guid> ids) =>
        _context.Tickets
            .Include(t => t.PriceTier)
            .Where(t => ids.Contains(t.TicketId));

    public async Task AddAsync(Ticket entity, CancellationToken ct = default)
    {
      _context.Tickets.Add(entity);
      await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Ticket entity, CancellationToken ct = default)
    {
      _context.Tickets.Update(entity);
      await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
      var e = await _context.Tickets.FirstOrDefaultAsync(x => x.TicketId == id, ct);
      if (e is null) return;
      _context.Tickets.Remove(e);
      await _context.SaveChangesAsync(ct);
    }

    public async Task MarkSoldAsync(IEnumerable<Ticket> tickets, CancellationToken ct = default)
    {
      foreach (var t in tickets) t.Status = "sold";
      _context.Tickets.UpdateRange(tickets);
      await _context.SaveChangesAsync(ct);
    }
  }
}