using DirectoryService.Contracts.Location;
using DirectoryService.Core.Locations;
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

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateLocationDto location, CancellationToken cancellationToken)
    {
        var locationId = await _locationService.Create(location, cancellationToken);

        return Ok(locationId);
    }

    [HttpPatch]
    public async Task<IActionResult> Update([FromBody] UpdateLocationDto location, CancellationToken cancellationToken)
    {
        var locationResult = await _locationService.Update(location, cancellationToken);
        if (locationResult.IsFailure)
        {
            return BadRequest(locationResult.Error);
        }

        return Ok(locationResult.Value);
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
