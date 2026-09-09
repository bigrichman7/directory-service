using CSharpFunctionalExtensions;
using DirectoryService.Core.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.ValueObjects;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Infrastructure.Postgres.Repositories;

public class DepartmentsRepository : IDepartmentsRepository
{
    private readonly DirectoryServiceDbContext _dbContext;
    private readonly ILogger<DepartmentsRepository> _logger;

    public DepartmentsRepository(DirectoryServiceDbContext dbContext, ILogger<DepartmentsRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<Guid> AddAsync(Department department, CancellationToken cancellationToken)
    {
        await _dbContext.Departments.AddAsync(department, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Новый отдел с Id {DepartmentId} добавлен", department.Id.Value);
        return department.Id.Value;
    }

    public async Task<Guid> UpdateAsync(Department department, CancellationToken cancellationToken)
    {
        _dbContext.Departments.Update(department);
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Отдел с Id {DepartmentId} обновлен", department.Id.Value);
        return department.Id.Value;
    }

    public async Task<Result<Guid, Error>> AddDepartmentWithDepartmentLocationAsync(Department department, IEnumerable<DepartmentLocation> departmentLocations, CancellationToken cancellationToken)
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

            _logger.LogInformation("Новый отдел с Id {DepartmentId} и его связи с локациями добавлены", department.Id.Value);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            _logger.LogError(ex, "Ошибка при добавлении отдела и его связей с локациями");
            return Error.Failure("directory.department.insert", $"Ошибка при добавлении отдела и его связей с локациями");
        }

        return department.Id.Value;
    }

    public async Task<Result<Department, Error>> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
        {
            _logger.LogWarning("Отдел с Id {DepartmentId} не найден", id);
            return Error.NotFound("directory.department.not_found", $"Отдел с Id {id} не найден");
        }

        return department;
    }

    public async Task<Result<Guid, Error>> GetByNameAsync(Name name, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .Where(x => x.Name == name)
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
        {
            _logger.LogWarning("Отдел с именем {Name} не найден", name);
            return Error.NotFound("directory.department.not_found", $"Отдел с именем {name} не найден");
        }

        return department.Id.Value;
    }

    public async Task<Result<Guid, Error>> GetBySlugAsync(Slug slug, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .Where(x => x.Slug == slug)
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
        {
            _logger.LogWarning("Отдел с slug {Slug} не найден", slug);
            return Error.NotFound("directory.department.not_found", $"Отдел с slug {slug} не найден");
        }

        return department.Id.Value;
    }

    public async Task<Result<Domain.ValueObjects.Path, Error>> GetPathByIdAsync(DepartmentId id, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
        {
            _logger.LogWarning("Отдел с Id {DepartmentId} не найден", id);
            return Error.NotFound("directory.department.not_found", $"Отдел с Id {id} не найден");
        }

        if (department.Path is null)
        {
            _logger.LogWarning("Путь для отдела с Id {DepartmentId} не найден", id);
            return Error.NotFound("directory.department.path_not_found", $"Путь для отдела с Id {id} не найден");
        }

        return department.Path;
    }

    public async Task<Result<DepartmentLocation, Error>> AddDepartmentLocationAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken)
    {
        var existingDepartmentLocation = await _dbContext.DepartmentLocations
            .Where(dl => dl.DepartmentId == departmentLocation.DepartmentId && dl.LocationId == departmentLocation.LocationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingDepartmentLocation is not null)
        {
            _logger.LogError("Связь отдела с Id {DepartmentId} и локации с Id {LocationId} уже существует", departmentLocation.DepartmentId, departmentLocation.LocationId);
            return Error.Conflict("directory.department_location.already_exists", $"Связь отдела с Id {departmentLocation.DepartmentId} и локации с Id {departmentLocation.LocationId} уже существует");
        }

        await _dbContext.DepartmentLocations.AddAsync(departmentLocation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Связь отдела с Id {DepartmentId} и локации с Id {LocationId} добавлена", departmentLocation.DepartmentId, departmentLocation.LocationId);

        return departmentLocation;
    }

    public async Task<Result<DepartmentLocation, Error>> RemoveDepartmentLocationAsync(DepartmentId departmentId, LocationId locationId, CancellationToken cancellationToken)
    {
        var existingDepartmentLocation = await _dbContext.DepartmentLocations
            .Where(dl => dl.DepartmentId == departmentId && dl.LocationId == locationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingDepartmentLocation is null)
        {
            _logger.LogError("Связь отдела с Id {DepartmentId} и локации с Id {LocationId} не найдена", departmentId, locationId);
            return Error.NotFound("directory.department_location.not_found", $"Связь отдела с Id {departmentId} и локации с Id {locationId} не найдена");
        }

        _dbContext.DepartmentLocations.Remove(existingDepartmentLocation);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Связь отдела с Id {DepartmentId} и локации с Id {LocationId} удалена", departmentId, locationId);

        return existingDepartmentLocation;
    }
}