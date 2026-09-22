using DirectoryService.Contracts.Location;
using DirectoryService.Core.Locations;
using DirectoryService.Web.EndpointResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
    [ProducesResponseType(typeof(Envelope<LocationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<LocationResponse>> Get(CancellationToken cancellation)
    {
        var dummy = new LocationResponse(
            Guid.Empty,
            "Тестовая локация (заглушка)",
            "Тестовый адрес",
            DateTime.UtcNow,
            DateTime.UtcNow);
        
        return new EndpointResult<LocationResponse>(
            dummy,
            statusCode: (int)HttpStatusCode.OK);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<Guid>> Create([FromBody] CreateLocationDto location, CancellationToken cancellationToken)
    {
        var result = await _locationService.Create(location, cancellationToken);

        return new EndpointResult<Guid>(
            result,
            statusCode: (int)HttpStatusCode.Created);
    }

    [HttpPatch("{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope<LocationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<LocationResponse>> UpdateLocation([FromRoute] Guid locationId, [FromBody] UpdateLocationDto location, CancellationToken cancellationToken)
    {
        return await _locationService.Update(locationId, location, cancellationToken);
    }

    [HttpPut("{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope<LocationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<LocationResponse>> Update([FromRoute] Guid locationId, [FromBody] UpdateLocationDto location, CancellationToken cancellationToken)
    {
        return await _locationService.Update(locationId, location, cancellationToken);
    }

    [HttpDelete("{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<Guid>> Delete([FromRoute] Guid locationId, CancellationToken cancellation)
    {
        return new EndpointResult<Guid>(
            Guid.Empty,
            statusCode: (int)HttpStatusCode.OK);
    }
}
