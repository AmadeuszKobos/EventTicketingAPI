using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
  public class Event
  {
    public Guid EventId { get; set; }
    public Guid VenueId { get; set; }
    public string Name { get; set; } = default!;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string? Description { get; set; }

    public Venue? Venue { get; set; }
    public ICollection<PriceTier> PriceTiers { get; set; } = new List<PriceTier>();
    public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
  }
}
