using Domain;

namespace Repositories.Repos
{
  public interface ICustomerRepository : IBaseRepository<Customer>
  {
    new IQueryable<Customer> GetAll();
    IQueryable<Customer> GetById(Guid id);
    Task<bool> ExistsAsync(Guid id, CancellationToken ct = default);

  }
}
