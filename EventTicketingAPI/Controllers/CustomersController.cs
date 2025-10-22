using Microsoft.AspNetCore.Mvc;
using Repositories.Repos;
using Services.Services;

namespace EventTicketingAPI.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")]
  public class CustomersController : ControllerBase
  {
    private readonly ICustomerService _customerService;
    public CustomersController(ICustomerService customerService) => _customerService = customerService;

    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<CustomerReadDto>>> GetAll()
    {
      var customers = await _customerService.GetAllAsync();
      return Ok(customers);
    }

    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<CustomerReadDto>> GetById(Guid id)
    {
      var customer = await _customerService.GetByIdAsync(id);
      return customer == null ? NotFound() : Ok(customer);
    }
  }
}
