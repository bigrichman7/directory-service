using DirectoryService.Contracts.Department;
using DirectoryService.Core.Departments;
using DirectoryService.Web.EndpointResults;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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
    [ProducesResponseType(typeof(Envelope<DepartmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<DepartmentResponse>> Get(CancellationToken cancellation)
    {
        var dummy = new DepartmentResponse(
            Id: Guid.Empty,
            ParentId: Guid.Empty,
            Name: "Тестовый отдел (заглушка)",
            Slug: "TEST",
            Path: "TEST",
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: DateTime.UtcNow);

        return new EndpointResult<DepartmentResponse>(
            dummy,
            statusCode: (int)HttpStatusCode.OK);
    }

    [HttpGet("{departmentId:guid}")]
    [ProducesResponseType(typeof(Envelope<DepartmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<DepartmentResponse>> GetById([FromRoute] Guid departmentId, CancellationToken cancellationToken) 
    {
        var dummy = new DepartmentResponse(
            Id: Guid.Empty,
            ParentId: Guid.Empty,
            Name: "Тестовый отдел (заглушка)",
            Slug: "TEST",
            Path: "TEST",
            CreatedAt: DateTime.UtcNow,
            UpdatedAt: DateTime.UtcNow);

        return new EndpointResult<DepartmentResponse>(
            dummy,
            statusCode: (int)HttpStatusCode.OK);
    }

    [HttpPost]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<Guid>> Create([FromBody] CreateDepartmentDto department, CancellationToken cancellationToken)
    {
        var result = await _departmentsService.Create(department, cancellationToken);

        return new EndpointResult<Guid>(
            result,
            statusCode: (int)HttpStatusCode.Created);
    }

    [HttpPost("{departmentId:guid}/locations/{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope<DepartmentLocationResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status409Conflict)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<DepartmentLocationResponse>> AddLocationToDepartment([FromRoute] Guid departmentId, [FromRoute] Guid locationId, [FromBody] bool? isPrimary, CancellationToken cancellationToken)
    {
        var result = await _departmentsService.AddLocation(departmentId, locationId, isPrimary ?? false, cancellationToken);

        return new EndpointResult<DepartmentLocationResponse>(
            result,
            statusCode: (int)HttpStatusCode.Created);
    }

    [HttpDelete("{departmentId:guid}/locations/{locationId:guid}")]
    [ProducesResponseType(typeof(Envelope<DepartmentLocationResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<DepartmentLocationResponse>> RemoveLocationFromDepartment([FromRoute] Guid departmentId, [FromRoute] Guid locationId, CancellationToken cancellationToken)
    {
        return await _departmentsService.RemoveLocation(departmentId, locationId, cancellationToken);
    }

    [HttpPatch("{departmentId:guid}")]
    [ProducesResponseType(typeof(Envelope<DepartmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<DepartmentResponse>> UpdateDepartment([FromRoute] Guid departmentId, [FromBody] UpdateDepartmentDto departmentDto, CancellationToken cancellation)
    {
        return await _departmentsService.Update(departmentId, departmentDto, cancellation);
    }

    [HttpPut("{departmentId:guid}")]
    [ProducesResponseType(typeof(Envelope<DepartmentResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<DepartmentResponse>> Update([FromRoute] Guid departmentId, [FromBody] UpdateDepartmentDto departmentDto, CancellationToken cancellation)
    {
        return await _departmentsService.Update(departmentId, departmentDto, cancellation);
    }

    [HttpDelete("{departmentId:guid}")]
    [ProducesResponseType(typeof(Envelope<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(Envelope), StatusCodes.Status500InternalServerError)]
    public async Task<EndpointResult<Guid>> Delete([FromRoute] Guid departmentId, CancellationToken cancellation)
    {
        return new EndpointResult<Guid>(
            Guid.Empty,
            statusCode: (int)HttpStatusCode.OK);
    }
}
