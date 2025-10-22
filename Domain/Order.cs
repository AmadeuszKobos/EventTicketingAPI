using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
  public class Order
  {
    public Guid OrderId { get; set; }
    public Guid CustomerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public decimal TotalAmount { get; set; }

    public Customer? Customer { get; set; }
    public ICollection<OrderTicket> OrderTickets { get; set; } = new List<OrderTicket>();
  }
}
