namespace Repositories.Repos
{
  public class VenueReadDto
  {
    public Guid VenueId { get; set; }
    public string Name { get; set; } = default!;
    public string City { get; set; } = default!;
    public int Capacity { get; set; }
  }
}