using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Repos;

namespace Services.Services
{
  public class PriceTierService : IPriceTierService
  {
    private readonly IPriceTierRepository _tiers;
    private readonly IEventRepository _events;

    public PriceTierService(IPriceTierRepository tiers, IEventRepository events)
    {
      _tiers = tiers;
      _events = events;
    }

    public async Task<List<PriceTierReadDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _tiers.GetAll().ToListAsync(ct)).Select(Mapper.Map).ToList();

    public async Task<PriceTierReadDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
      var e = await _tiers.GetById(id).FirstOrDefaultAsync(ct);
      return e is null ? null : Mapper.Map(e);
    }

    public async Task<PriceTierReadDto> CreateAsync(PriceTierCreateDto dto, CancellationToken ct = default)
    {
      if (dto.Amount < 0) throw new ValidationException("Amount must be >= 0.");
      var eventExists = await _events.GetById(dto.EventId).AnyAsync(ct);
      if (!eventExists) throw new NotFoundException($"Event {dto.EventId} not found.");

      var entity = Mapper.Map(dto);
      await _tiers.AddAsync(entity, ct);
      var saved = await _tiers.GetById(entity.PriceTierId).FirstAsync(ct);
      return Mapper.Map(saved);
    }

    public async Task UpdateAsync(Guid id, PriceTierUpdateDto dto, CancellationToken ct = default)
    {
      if (dto.Amount < 0) throw new ValidationException("Amount must be >= 0.");

      var existing = await _tiers.GetById(id).AsTracking().FirstOrDefaultAsync(ct);
      if (existing is null) throw new NotFoundException($"PriceTier {id} not found.");

      Mapper.Map(dto, existing);
      await _tiers.UpdateAsync(existing, ct);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default) =>
        _tiers.DeleteAsync(id, ct);
  }
}
