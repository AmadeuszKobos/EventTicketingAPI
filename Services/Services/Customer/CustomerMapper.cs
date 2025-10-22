using Domain;
using Services.Services;

namespace Repositories.Repos
{
  public static partial class Mapper
  {
    public static CustomerReadDto Map(Customer c) => new()
    {
      CustomerId = c.CustomerId,
      FullName = c.FullName,
      Email = c.Email
    };

    public static Customer Map(CustomerCreateDto dto) => new()
    {
      CustomerId = Guid.NewGuid(),
      FullName = dto.FullName,
      Email = dto.Email
    };
  }
}
