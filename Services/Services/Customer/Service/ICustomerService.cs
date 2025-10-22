using Repositories.Repos;

namespace Services.Services
{
  public interface ICustomerService
  {
    Task<List<CustomerReadDto>> GetAllAsync();
    Task<CustomerReadDto?> GetByIdAsync(Guid id);
  }
}
