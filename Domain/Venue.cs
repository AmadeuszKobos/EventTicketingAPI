using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
  public class Venue
  {
    public Guid VenueId { get; set; }
    public string Name { get; set; } = default!;
    public string City { get; set; } = default!;
    public int Capacity { get; set; }

    public ICollection<Event> Events { get; set; } = new List<Event>();
  }
}