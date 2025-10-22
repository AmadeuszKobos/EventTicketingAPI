using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repos
{
  public class VenueRepository : BaseRepository<Venue>, IVenueRepository
  {
    public VenueRepository(AppDbContext context) : base(context)
    {
    }

    public IQueryable<Venue> GetAll() =>
        _context.Venues.AsNoTracking().Include(v => v.Events);

    public IQueryable<Venue> GetById(Guid id) =>
        _context.Venues.AsNoTracking().Include(v => v.Events)
            .Where(v => v.VenueId == id);
  }
}