using Domain;
using Microsoft.EntityFrameworkCore;
using Repositories.Repos;

namespace Services.Services
{
  public class TicketService : ITicketService
  {
    private readonly ITicketRepository _tickets;
    private readonly IEventRepository _events;
    private readonly IPriceTierRepository _tiers;

    public TicketService(ITicketRepository tickets, IEventRepository events, IPriceTierRepository tiers)
    {
      _tickets = tickets;
      _events = events;
      _tiers = tiers;
    }

    public async Task<List<TicketReadDto>> GetAllAsync(CancellationToken ct = default) =>
        (await _tickets.GetAll().ToListAsync(ct)).Select(Mapper.MapTicket).ToList();

    public async Task<TicketReadDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
      var e = await _tickets.GetById(id).FirstOrDefaultAsync(ct);
      return e is null ? null : Mapper.MapTicket(e);
    }

    public async Task<TicketReadDto> CreateAsync(TicketCreateDto dto, CancellationToken ct = default)
    {
      if (string.IsNullOrWhiteSpace(dto.Code))
        throw new ValidationException("Code is required.");

      var eventExists = await _events.GetById(dto.EventId).AnyAsync(ct);
      if (!eventExists) throw new NotFoundException($"Event {dto.EventId} not found.");

      var tierExists = await _tiers.GetById(dto.PriceTierId).AnyAsync(ct);
      if (!tierExists) throw new NotFoundException($"PriceTier {dto.PriceTierId} not found.");

      var entity = Mapper.Map(dto);
      await _tickets.AddAsync(entity, ct);

      var saved = await _tickets.GetById(entity.TicketId).FirstAsync(ct);
      return Mapper.MapTicket(saved);
    }

    public async Task UpdateAsync(Guid id, TicketUpdateDto dto, CancellationToken ct = default)
    {
      var valid = new[] { "available", "reserved", "sold" };
      if (!valid.Contains(dto.Status)) throw new ValidationException("Invalid status.");

      var existing = await _tickets.GetById(id).AsTracking().FirstOrDefaultAsync(ct);
      if (existing is null) throw new NotFoundException($"Ticket {id} not found.");

      Mapper.Map(dto, existing);
      await _tickets.UpdateAsync(existing, ct);
    }

    public Task DeleteAsync(Guid id, CancellationToken ct = default) =>
        _tickets.DeleteAsync(id, ct);
  }
}