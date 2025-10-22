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
  public class VenuesController : ControllerBase
  {
    private readonly IVenueService _service;

    public VenuesController(IVenueService service) => _service = service;

    [HttpGet("GetAll")]
    public async Task<ActionResult<IEnumerable<VenueReadDto>>> GetAll(CancellationToken ct) =>
        Ok(await _service.GetAllAsync(ct));

    [HttpGet("GetById/{id:guid}")]
    public async Task<ActionResult<VenueReadDto>> GetById(Guid id, CancellationToken ct)
    {
      var dto = await _service.GetByIdAsync(id, ct);
      return dto is null ? NotFound() : Ok(dto);
    }
  }
}