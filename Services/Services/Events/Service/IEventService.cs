using Repositories.Repos;
using Repositories.Repos.Events;

namespace Services.Services
{
  public interface IEventService
  {
    Task<List<EventReadDto>> GetAllAsync(CancellationToken ct = default);
    Task<EventReadDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<EventReadDto> CreateAsync(EventCreateDto dto, CancellationToken ct = default);
    Task UpdateAsync(Guid id, EventUpdateDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);

  }
}
