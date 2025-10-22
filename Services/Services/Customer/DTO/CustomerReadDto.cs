using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repos
{
  public class CustomerReadDto
  {
    public Guid CustomerId { get; set; }
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
  }
}
