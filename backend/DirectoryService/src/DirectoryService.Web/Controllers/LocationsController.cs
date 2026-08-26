using DirectoryService.Contracts.Location;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Locations;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("api/locations")]
public class LocationsController : ControllerBase
{
    private readonly ILocationsService _locationService;

    public LocationsController(ILocationsService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellation)
    {
        return Ok(new List<object>());
    }

    [HttpGet("{locationId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        return NotFound("Location didn't be found");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLocationDto location, CancellationToken cancellationToken)
    {
        var locationId = await _locationService.Create(location, cancellationToken);

        return Ok(locationId);
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
