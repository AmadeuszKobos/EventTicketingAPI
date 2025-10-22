using Repositories.Repos;

namespace Services.Services
{
  public interface ITicketService
  {
    Task<List<TicketReadDto>> GetAllAsync(CancellationToken ct = default);

    Task<TicketReadDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<TicketReadDto> CreateAsync(TicketCreateDto dto, CancellationToken ct = default);

    Task UpdateAsync(Guid id, TicketUpdateDto dto, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
  }
}