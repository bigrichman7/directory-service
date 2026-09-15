using CSharpFunctionalExtensions;
using DirectoryService.Core.Departments;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.ValueObjects;
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
        await _dbContext.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Отдел с Id {DepartmentId} обновлен", department.Id.Value);
        return department.Id.Value;
    }

    public async Task<Guid?> AddDepartmentWithDepartmentLocationAsync(Department department, IEnumerable<DepartmentLocation> departmentLocations, CancellationToken cancellationToken)
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
        }

        return department.Id.Value;
    }

    public async Task<Department?> GetByIdAsync(DepartmentId id, CancellationToken cancellationToken)
    {
        return await _dbContext.Departments
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Department?> GetByNameAsync(Name name, CancellationToken cancellationToken)
    {
        return await _dbContext.Departments
            .Where(x => x.Name == name)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Department?> GetBySlugAsync(Slug slug, CancellationToken cancellationToken)
    {
        return await _dbContext.Departments
            .Where(x => x.Slug == slug)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<DepartmentLocation?> AddDepartmentLocationAsync(DepartmentLocation departmentLocation, CancellationToken cancellationToken)
    {
        var existingDepartmentLocation = await _dbContext.DepartmentLocations
            .Where(dl => dl.DepartmentId == departmentLocation.DepartmentId && dl.LocationId == departmentLocation.LocationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingDepartmentLocation is not null)
        {
            _logger.LogError("Связь отдела с Id {DepartmentId} и локации с Id {LocationId} уже существует", departmentLocation.DepartmentId, departmentLocation.LocationId);
            return null;
        }

        await _dbContext.DepartmentLocations.AddAsync(departmentLocation, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Связь отдела с Id {DepartmentId} и локации с Id {LocationId} добавлена", departmentLocation.DepartmentId, departmentLocation.LocationId);

        return departmentLocation;
    }

    public async Task<DepartmentLocation?> RemoveDepartmentLocationAsync(DepartmentId departmentId, LocationId locationId, CancellationToken cancellationToken)
    {
        var existingDepartmentLocation = await _dbContext.DepartmentLocations
            .Where(dl => dl.DepartmentId == departmentId && dl.LocationId == locationId)
            .FirstOrDefaultAsync(cancellationToken);

        if (existingDepartmentLocation is null)
        {
            _logger.LogError("Связь отдела с Id {DepartmentId} и локации с Id {LocationId} не найдена", departmentId, locationId);
            return null;
        }

        _dbContext.DepartmentLocations.Remove(existingDepartmentLocation);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Связь отдела с Id {DepartmentId} и локации с Id {LocationId} удалена", departmentId, locationId);

        return existingDepartmentLocation;
    }
}