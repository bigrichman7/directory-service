using DirectoryService.Contracts.Location;
using DirectoryService.Core.Locations;
using Microsoft.AspNetCore.Mvc;
using Shared;

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
        var result = await _locationService.Create(location, cancellationToken);
        if(result.IsFailure)
        {
            return result.Error.ToFailure().ToResponse();
        }

        return Ok(result.Value);
    }

    [HttpPatch("{locationId:guid}")]
    public async Task<IActionResult> UpdateLocation([FromRoute] Guid locationId, [FromBody] UpdateLocationDto location, CancellationToken cancellationToken)
    {
        var result = await _locationService.Update(locationId, location, cancellationToken);
        if(result.IsFailure)
        {
            return result.Error.ToFailure().ToResponse();
        }

        return Ok(result.Value);
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
