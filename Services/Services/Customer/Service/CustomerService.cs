using Microsoft.EntityFrameworkCore;
using Repositories.Repos;


namespace Services.Services
{
  public class CustomerService : ICustomerService
  {
    private readonly ICustomerRepository _repo;
    public CustomerService(ICustomerRepository repo) => _repo = repo;

    public async Task<List<CustomerReadDto>> GetAllAsync()
    {
      var customers = await _repo.GetAll().ToListAsync();
      return customers.Select(Mapper.Map).ToList();
    }

    public async Task<CustomerReadDto?> GetByIdAsync(Guid id)
    {
      var entity = await _repo.GetById(id).FirstOrDefaultAsync();
      return entity == null ? null : Mapper.Map(entity);
    }
  }
}
