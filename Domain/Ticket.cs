using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
  public class Ticket
  {
    public Guid TicketId { get; set; }
    public Guid EventId { get; set; }
    public Guid PriceTierId { get; set; }
    public string Status { get; set; } = "available"; // available|reserved|sold
    public string Code { get; set; } = default!;

    public Event? Event { get; set; }
    public PriceTier? PriceTier { get; set; }
    public ICollection<OrderTicket> OrderTickets { get; set; } = new List<OrderTicket>();
  }
}