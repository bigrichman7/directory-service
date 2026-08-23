using DirectoryService.Contracts.Position;
using DirectoryService.Domain.Positions;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("/api/positions")]
public class PositionsController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellation)
    {
        return Ok(new List<object>());
    }

    [HttpGet("{positionId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid positionId, CancellationToken cancellationToken)
    {
        return NotFound("Position didn't be found");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePositionDto position, CancellationToken cancellationToken)
    {
        return Ok("Position created");
    }

    [HttpPut("{positionId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid positionId, [FromBody] UpdatePositionDto position, CancellationToken cancellation)
    {
        return Ok("Position updated");
    }

    [HttpDelete("{positionId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid positionId, CancellationToken cancellation)
    {
        return Ok("Position deleted");
    }
}

