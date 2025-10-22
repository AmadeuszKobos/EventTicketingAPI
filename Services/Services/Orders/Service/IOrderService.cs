using Repositories.Repos;

namespace Services.Services
{
  public interface IOrderService 
  {
    Task<List<OrderReadDto>> GetAllAsync(CancellationToken ct = default);
    Task<OrderReadDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<OrderReadDto> CreateAsync(OrderCreateDto dto, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
  }
}
