using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Services
{
  public class CustomerCreateDto
  {
    public string FullName { get; set; } = default!;
    public string Email { get; set; } = default!;
  }
}
