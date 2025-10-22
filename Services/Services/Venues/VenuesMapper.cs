using Domain;

namespace Repositories.Repos
{
  public partial class Mapper
  {
    public static VenueReadDto Map(Venue v) => new()
    {
      VenueId = v.VenueId,
      Name = v.Name,
      City = v.City,
      Capacity = v.Capacity
    };
  }
}
