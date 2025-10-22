using Domain;

namespace Repositories.Repos
{
  public interface IVenueRepository : IBaseRepository<Venue>
  {
    IQueryable<Venue> GetAll();

    IQueryable<Venue> GetById(Guid id);
  }
}