namespace Repositories.Repos
{
  public class EventCreateDto
  {
    public Guid VenueId { get; set; }
    public string Name { get; set; } = default!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Description { get; set; }
  }
}
