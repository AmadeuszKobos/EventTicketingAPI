namespace Repositories.Repos.Events
{
  public class EventReadDto
  {
    public Guid EventId { get; set; }
    public Guid VenueId { get; set; }
    public string Name { get; set; } = default!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Description { get; set; }
    public string VenueName { get; set; } = "";
  }
}
