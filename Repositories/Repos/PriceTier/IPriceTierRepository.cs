using Domain;

namespace Repositories.Repos
{
  public interface IPriceTierRepository : IBaseRepository<PriceTier>
  {
    new IQueryable<PriceTier> GetAll();
    IQueryable<PriceTier> GetById(Guid id);
    Task AddAsync(PriceTier entity, CancellationToken ct = default);
    Task UpdateAsync(PriceTier entity, CancellationToken ct = default);
    Task DeleteAsync(Guid id, CancellationToken ct = default);
  }
}
