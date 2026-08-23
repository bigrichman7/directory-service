using DirectoryService.Contracts.Department;
using DirectoryService.Domain.Departments;
using Microsoft.AspNetCore.Mvc;

namespace DirectoryService.Web.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
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
        return Ok("Department created");
    }

    [HttpPut("{departmentId:guid}")]
    public async Task<IActionResult> Update([FromRoute] Guid departmentId, [FromBody] UpdateDepartmentDto department, CancellationToken cancellation)
    {
        return Ok("Department updated");
    }

    [HttpDelete("{departmentId:guid}")]
    public async Task<IActionResult> Delete([FromRoute] Guid departmentId, CancellationToken cancellation)
    {
        return Ok("Department deleted");
    }
}
