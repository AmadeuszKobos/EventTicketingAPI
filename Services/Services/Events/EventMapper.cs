using Domain;
using Repositories.Repos.Events;
using Services.Services;

namespace Repositories.Repos
{
  public static partial class Mapper
  {
    public static EventReadDto Map(Event e) => new()
    {
      EventId = e.EventId,
      VenueId = e.VenueId,
      Name = e.Name,
      StartTime = e.StartTime,
      EndTime = e.EndTime,
      Description = e.Description,
      VenueName = e.Venue?.Name ?? ""
    };

    public static Event Map(EventCreateDto dto) => new()
    {
      EventId = Guid.NewGuid(),
      VenueId = dto.VenueId,
      Name = dto.Name,
      StartTime = dto.StartTime,
      EndTime = dto.EndTime,
      Description = dto.Description
    };

    public static void Map(EventUpdateDto dto, Event entity)
    {
      entity.VenueId = dto.VenueId;
      entity.Name = dto.Name;
      entity.StartTime = dto.StartTime;
      entity.EndTime = dto.EndTime;
      entity.Description = dto.Description;
    }
  }
}
