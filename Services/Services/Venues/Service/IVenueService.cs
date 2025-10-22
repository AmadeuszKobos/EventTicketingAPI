using Repositories.Repos;

namespace Services.Services
{
  public interface IVenueService
  {
    Task<List<VenueReadDto>> GetAllAsync(CancellationToken ct = default);

    Task<VenueReadDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
  }
}