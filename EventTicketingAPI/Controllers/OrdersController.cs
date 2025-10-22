using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using Repositories.Repos;
using Services.Services;

namespace EventTicketingAPI.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")]
  public class OrdersController : ControllerBase
  {
    private readonly IOrderService _service;
    public OrdersController(IOrderService service) => _service = service;

    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<OrderReadDto>>> GetAll(CancellationToken ct)
    {
      var items = await _service.GetAllAsync(ct);
      return Ok(items);
    }

    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<OrderReadDto>> GetById(Guid id, CancellationToken ct)
    {
      var dto = await _service.GetByIdAsync(id, ct);
      return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("Create")]
    public async Task<ActionResult<OrderReadDto>> Create([FromBody] OrderCreateDto dto, CancellationToken ct)
    {
      var created = await _service.CreateAsync(dto, ct);
      return CreatedAtAction(nameof(GetById), new { id = created.OrderId }, created);
    }

    [HttpDelete("Delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
      await _service.DeleteAsync(id, ct);
      return NoContent();
    }
  }
}
