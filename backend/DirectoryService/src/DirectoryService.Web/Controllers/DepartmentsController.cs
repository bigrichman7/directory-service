using DirectoryService.Contracts.Department;
using DirectoryService.Core.Departments;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentsService _departmentsService;

    public DepartmentsController(IDepartmentsService departmentsService)
    {
        _departmentsService = departmentsService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellation)
    {
        return Ok(new List<object>());
    }

    [HttpGet("{departmentId:guid}")]
    public async Task<IActionResult> GetById([FromRoute] Guid departmentId, CancellationToken cancellationToken) 
    {
        return NotFound("Department didn't be found");
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentDto department, CancellationToken cancellationToken)
    {
        var result = await _departmentsService.Create(department, cancellationToken);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<IActionResult> AddLocationToDepartment([FromRoute] Guid departmentId, [FromRoute] Guid locationId, [FromBody] bool? isPrimary, CancellationToken cancellationToken)
    {
        var result = await _departmentsService.AddLocation(departmentId, locationId, isPrimary ?? false, cancellationToken);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    public async Task<IActionResult> RemoveLocationFromDepartment([FromRoute] Guid departmentId, [FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        var result = await _departmentsService.RemoveLocation(departmentId, locationId, cancellationToken);
        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPatch("{departmentId:guid}")]
    public async Task<IActionResult> UpdateDepartment([FromRoute] Guid departmentId, [FromBody] UpdateDepartmentDto departmentDto, CancellationToken cancellation)
    {
        var result = await _departmentsService.Update(departmentId, departmentDto, cancellation);

        if (result.IsFailure)
            return BadRequest(result.Error);

        return Ok(result.Value);
    }

    [HttpPut("{departmentId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid departmentId, [FromBody] UpdateDepartmentDto departmentDto, CancellationToken cancellation)
    {
        return Ok("Department updated");
    }

    [HttpDelete("{departmentId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid departmentId, CancellationToken cancellation)
    {
        return Ok("Department deleted");
    }
}
