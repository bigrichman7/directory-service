using CSharpFunctionalExtensions;
using DirectoryService.Core.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.ValueObjects;
using DirectoryService.Infrastructure.Postgres.Exceptions;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class DepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;

    public DepartmentsRepository(DirectoryServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Guid> AddAsync(Department department, CancellationToken cancellationToken)
    {
        await _dbContext.Departments.AddAsync(department, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return department.Id.Value;
    }

    public async Task<Guid> UpdateAsync(Department department, CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
        return department.Id.Value;
    }

    public async Task<Guid> AddDepartmentWithDepartmentLocationAsync(Department department, IEnumerable<DepartmentLocation> departmentLocations, CancellationToken cancellationToken)
    {
        using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            await _dbContext.Departments.AddAsync(department, cancellationToken);
            foreach (var departmentLocation in departmentLocations)
            {
                await _dbContext.DepartmentLocations.AddAsync(departmentLocation, cancellationToken);
            }

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new InfrastructureBadRequestException(Error.Failure("department.add_failed", $"Не удалось добавить отдел с Id {department.Id.Value} и его связи с локациями"));
        }

        return department.Id.Value;
    }

    public async Task<Result<Department, Error>> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        return department is not null
            ? department : Error.NotFound("id.not_found", $"Отдел с Id {id.Value} не найден");
    }

    public async Task<Result<Department, Error>> GetByNameAsync(Name name, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .Where(x => x.Name == name)
            .FirstOrDefaultAsync(cancellationToken);

        return department is not null
            ? department : Error.NotFound("name.not_found", $"Отдел с именем {name.Value} не найден");
    }

    public async Task<Result<Department, Error>> GetBySlugAsync(Slug slug, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .Where(x => x.Slug == slug)
            .FirstOrDefaultAsync(cancellationToken);

        return department is not null
            ? department : Error.NotFound("slug.not_found", $"Отдел с slug {slug.Value} не найден");
    }

    public async Task<Result<DepartmentLocation, Error>> AddDepartmentLocationAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken)
    {
        var existingDepartmentLocation = await _dbContext.DepartmentLocations
            .Where(dl => dl.DepartmentId == departmentLocation.DepartmentId && dl.LocationId == departmentLocation.LocationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingDepartmentLocation is not null)
        {
            return Error.Conflict("department_location.conflict", $"Связь отдела с Id {departmentLocation.DepartmentId} и локации с Id {departmentLocation.LocationId} уже существует");
        }

        await _dbContext.DepartmentLocations.AddAsync(departmentLocation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return departmentLocation;
    }

    public async Task<Result<DepartmentLocation, Error>> RemoveDepartmentLocationAsync(DepartmentId departmentId, LocationId locationId, CancellationToken cancellationToken)
    {
        var existingDepartmentLocation = await _dbContext.DepartmentLocations
            .Where(dl => dl.DepartmentId == departmentId && dl.LocationId == locationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingDepartmentLocation is null)
        {
            return Error.NotFound("department_location.not_found", $"Связь отдела с Id {departmentId} и локации с Id {locationId} не найдена");
        }

        _dbContext.DepartmentLocations.Remove(existingDepartmentLocation);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return existingDepartmentLocation;
    }
}