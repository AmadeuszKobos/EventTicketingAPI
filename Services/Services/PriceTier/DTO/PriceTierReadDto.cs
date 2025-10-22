using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repos
{
  public class PriceTierReadDto
  {
    public Guid PriceTierId { get; set; }
    public Guid EventId { get; set; }
    public string TierName { get; set; } = default!;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "USD";
    public string EventName { get; set; } = "";
  }
}