using Microsoft.AspNetCore.Mvc;
using Repositories.Repos;
using Repositories.Repos.Events;
using Services.Services;

namespace EventTicketingAPI.Controllers
{

  [ApiController]
  [Route("api/v1/[controller]")]
  public class EventsController : ControllerBase
  {
    private readonly IEventService _service;
    public EventsController(IEventService service) => _service = service;

    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<EventReadDto>>> GetAll(CancellationToken ct)
    {
      var items = await _service.GetAllAsync(ct);
      return Ok(items);
    }

    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<EventReadDto>> GetById(Guid id, CancellationToken ct)
    {
      var dto = await _service.GetByIdAsync(id, ct);
      return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("Create")]
    public async Task<ActionResult<EventReadDto>> Create([FromBody] EventCreateDto dto, CancellationToken ct)
    {
      var created = await _service.CreateAsync(dto, ct);
      return CreatedAtAction(nameof(GetById), new { id = created.EventId }, created);
    }

    [HttpPut("Update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] EventUpdateDto dto, CancellationToken ct)
    {
      await _service.UpdateAsync(id, dto, ct);
      return NoContent();
    }

    [HttpDelete("Delete/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
      await _service.DeleteAsync(id, ct);
      return NoContent();
    }
  }
}
