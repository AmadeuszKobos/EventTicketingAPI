using Microsoft.AspNetCore.Mvc;
using Repositories.Repos;
using Services.Services;

namespace EventTicketingAPI.Controllers
{
  [ApiController]
  [Route("api/v1/[controller]")]
  public class PriceTiersController : ControllerBase
  {
    private readonly IPriceTierService _service;

    public PriceTiersController(IPriceTierService service) => _service = service;

    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<PriceTierReadDto>>> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<PriceTierReadDto>> GetById(Guid id, CancellationToken ct)
    {
      var dto = await _service.GetByIdAsync(id, ct);
      return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost("Create")]
    public async Task<ActionResult<PriceTierReadDto>> Create([FromBody] PriceTierCreateDto dto, CancellationToken ct)
    {
      var created = await _service.CreateAsync(dto, ct);
      return CreatedAtAction(nameof(GetById), new { id = created.PriceTierId }, created);
    }

    [HttpPut("Update/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] PriceTierUpdateDto dto, CancellationToken ct)
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