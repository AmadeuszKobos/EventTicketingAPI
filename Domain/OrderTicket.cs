using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
  public class OrderTicket
  {
    public Guid OrderId { get; set; }
    public Guid TicketId { get; set; }
    public Order? Order { get; set; }
    public Ticket? Ticket { get; set; }
  }
}
