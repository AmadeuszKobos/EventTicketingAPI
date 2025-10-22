using Microsoft.EntityFrameworkCore;
using Repositories.Repos;

namespace Services.Services
{
  public class VenueService : IVenueService
  {
    private readonly IVenueRepository _venues;

    public VenueService(IVenueRepository venues) => _venues = venues;

    public async Task<List<VenueReadDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _venues.GetAll().ToListAsync(ct)).Select(Mapper.Map).ToList();

    public async Task<VenueReadDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
      var v = await _venues.GetById(id).FirstOrDefaultAsync(ct);
      return v is null ? null : Mapper.Map(v);
    }
  }
}