using DirectoryService.Contracts.Location;
using DirectoryService.Domain.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellation)
    {
        return Ok(new List<Location>());
    }

    [HttpGet("{locationId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        return NotFound("Location didn't be found");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLocationDto location, CancellationToken cancellationToken)
    {
        return Ok("Location created");
    }

    [HttpPut("{locationId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid locationId, [FromBody] UpdateLocationDto location, CancellationToken cancellation)
    {
        return Ok("Location updated");
    }

    [HttpDelete("{locationId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid locationId, CancellationToken cancellation)
    {
        return Ok("Location deleted");
    }
}
