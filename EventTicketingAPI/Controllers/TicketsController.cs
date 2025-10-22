using Microsoft.AspNetCore.Mvc;
using Repositories.Repos;
using Services.Services;

namespace EventTicketingAPI.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")]
  public class TicketsController : ControllerBase
  {
    private readonly ITicketService _service;

    public TicketsController(ITicketService service) => _service = service;

    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<TicketReadDto>>> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<TicketReadDto>> GetById(Guid id, CancellationToken ct)
    {
      var dto = await _service.GetByIdAsync(id, ct);
      return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("Create")]
    public async Task<ActionResult<TicketReadDto>> Create([FromBody] TicketCreateDto dto, CancellationToken ct)
    {
      var created = await _service.CreateAsync(dto, ct);
      return CreatedAtAction(nameof(GetById), new { id = created.TicketId }, created);
    }

    [HttpPut("Update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TicketUpdateDto dto, CancellationToken ct)
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