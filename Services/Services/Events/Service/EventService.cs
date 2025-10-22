using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Repos;
using Repositories.Repos.Events;

namespace Services.Services
{
  public class EventService : IEventService
  {
    private readonly IEventRepository _repo;
    public EventService(IEventRepository repo) => _repo = repo;

    public async Task<List<EventReadDto>> GetAllAsync(CancellationToken ct = default)
    {
      var entities = await _repo.GetAll().ToListAsync(ct);
      return entities.Select(Mapper.Map).ToList();
    }

    public async Task<EventReadDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
      var e = await _repo.GetById(id).FirstOrDefaultAsync(ct);
      return e is null ? null : Mapper.Map(e);
    }

    public async Task<EventReadDto> CreateAsync(EventCreateDto dto, CancellationToken ct = default)
    {
      if (dto.EndTime <= dto.StartTime)
        throw new Domain.ValidationException("EndTime must be after StartTime.");

      var entity = Mapper.Map(dto);
      await _repo.AddAsync(entity, ct);
      // reload with Venue for VenueName
      var saved = await _repo.GetById(entity.EventId).FirstAsync(ct);
      return Mapper.Map(saved);
    }

    public async Task UpdateAsync(Guid id, EventUpdateDto dto, CancellationToken ct = default)
    {
      if (dto.EndTime <= dto.StartTime)
        throw new ValidationException("EndTime must be after StartTime.");

      var existing = await _repo.GetById(id).AsTracking().FirstOrDefaultAsync(ct);
      if (existing is null) throw new NotFoundException($"Event {id} not found.");

      Mapper.Map(dto, existing);
      await _repo.UpdateAsync(existing, ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct = default)
    {
      await _repo.DeleteAsync(id, ct);
    }
  }
}
