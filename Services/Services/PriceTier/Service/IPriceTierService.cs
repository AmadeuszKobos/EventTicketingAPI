using Repositories.Repos;

namespace Services.Services
{
  public interface IPriceTierService
  {
    Task<List<PriceTierReadDto>> GetAllAsync(CancellationToken ct = default);

    Task<PriceTierReadDto?> GetByIdAsync(Guid id, CancellationToken ct = default);

    Task<PriceTierReadDto> CreateAsync(PriceTierCreateDto dto, CancellationToken ct = default);

    Task UpdateAsync(Guid id, PriceTierUpdateDto dto, CancellationToken ct = default);

    Task DeleteAsync(Guid id, CancellationToken ct = default);
  }
}