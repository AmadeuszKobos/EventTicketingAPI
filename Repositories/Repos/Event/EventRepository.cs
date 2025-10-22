using Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata.Ecma335;

namespace Repositories.Repos
{
  public class EventRepository : BaseRepository<Event>, IEventRepository
  {
    public EventRepository(AppDbContext context) : base(context) { }

    public new IQueryable<Event> GetAll() =>
        _context.Events
            .AsNoTracking()
            .Include(e => e.Venue);

    public IQueryable<Event> GetById(Guid id) =>
        _context.Events
            .AsNoTracking()
            .Include(e => e.Venue)
            .Where(e => e.EventId == id);

    public async Task AddAsync(Event entity, CancellationToken ct = default)
    {
      _context.Events.Add(entity);
      await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Event entity, CancellationToken ct = default)
    {
      _context.Events.Update(entity);
      await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
      var e = await _context.Events.FirstOrDefaultAsync(x => x.EventId == id, ct);
      if (e is null) return;
      _context.Events.Remove(e);
      await _context.SaveChangesAsync(ct);
    }
  }
}

