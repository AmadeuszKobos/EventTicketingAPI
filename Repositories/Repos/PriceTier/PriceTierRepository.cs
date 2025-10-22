using Domain;
using Microsoft.EntityFrameworkCore;

namespace Repositories.Repos
{
  public class PriceTierRepository : BaseRepository<PriceTier>, IPriceTierRepository
  {
    public PriceTierRepository(AppDbContext context) : base(context) { }

    public IQueryable<PriceTier> GetAll() =>
        _context.PriceTiers.AsNoTracking().Include(p => p.Event);

    public IQueryable<PriceTier> GetById(Guid id) =>
        _context.PriceTiers.AsNoTracking().Include(p => p.Event)
               .Where(p => p.PriceTierId == id);

    public async Task AddAsync(PriceTier entity, CancellationToken ct = default)
    {
      _context.PriceTiers.Add(entity);
      await _context.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(PriceTier entity, CancellationToken ct = default)
    {
      _context.PriceTiers.Update(entity);
      await _context.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
      var e = await _context.PriceTiers.FirstOrDefaultAsync(x => x.PriceTierId == id, ct);
      if (e is null) return;
      _context.PriceTiers.Remove(e);
      await _context.SaveChangesAsync(ct);
    }

  }
}
