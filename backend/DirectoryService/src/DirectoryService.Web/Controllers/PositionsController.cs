using DirectoryService.Contracts.Location;
using DirectoryService.Contracts.Position;
using DirectoryService.Web.EndpointResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("/api/positions")]
public class PositionsController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(Envelope<PositionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<PositionResponse>> Get(CancellationToken cancellation)
    {
        var dummy = new PositionResponse(
            Guid.Empty,
            "Тестовая должность (заглушка)",
            DateTime.UtcNow,
            DateTime.UtcNow);

        return new EndpointResult<PositionResponse>(
            dummy,
            (int)HttpStatusCode.OK);
    }

    [HttpGet("{positionId:guid}")]
    [ProducesResponseType(typeof(Envelope<PositionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<PositionResponse>> GetById([FromRoute] Guid positionId, CancellationToken cancellationToken)
    {
        var dummy = new PositionResponse(
            Guid.Empty,
            "Тестовая должность (заглушка)",
            DateTime.UtcNow,
            DateTime.UtcNow);

        return new EndpointResult<PositionResponse>(
            dummy,
            (int)HttpStatusCode.OK);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<Guid>> Create([FromBody] CreatePositionDto position, CancellationToken cancellationToken)
    {
        return new EndpointResult<Guid>(
            Guid.Empty,
            (int)HttpStatusCode.OK);
    }

    [HttpPut("{positionId:guid}")]
    [ProducesResponseType(typeof(Envelope<PositionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<PositionResponse>> Update([FromRoute] Guid positionId, [FromBody] UpdatePositionDto position, CancellationToken cancellation)
    {
        var dummy = new PositionResponse(
            Guid.Empty,
            "Тестовая должность (заглушка)",
            DateTime.UtcNow,
            DateTime.UtcNow);

        return new EndpointResult<PositionResponse>(
            dummy,
            (int)HttpStatusCode.OK);
    }

    [HttpDelete("{positionId:guid}")]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<Guid>> Delete([FromRoute] Guid positionId, CancellationToken cancellation)
    {
        return new EndpointResult<Guid>(
            Guid.Empty,
            (int)HttpStatusCode.OK);
    }
}

