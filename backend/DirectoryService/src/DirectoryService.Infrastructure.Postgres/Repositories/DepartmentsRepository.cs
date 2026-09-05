using CSharpFunctionalExtensions;
using DirectoryService.Core.Departments;
using DirectoryService.Domain.Departments;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

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

    public async Task<Result<Guid, Error>> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .Where(x => x.Id == new DepartmentId(id))
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
        {
            _logger.LogWarning("Отдел с Id {DepartmentId} не найден", id);
            return Error.NotFound("directory.department.not_found", $"Отдел с Id {id} не найден");
        }

        return department.Id.Value;
    }

    public async Task<Result<Guid, Error>> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .Where(x => x.Name.Value == name)
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
        {
            _logger.LogWarning("Отдел с именем {Name} не найден", name);
            return Error.NotFound("directory.department.not_found", $"Отдел с именем {name} не найден");
        }

        return department.Id.Value;
    }

    public async Task<Result<Guid, Error>> GetBySlugAsync(string slug, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .Where(x => x.Slug.Value == slug)
            .FirstOrDefaultAsync(cancellationToken);

        if (department is null)
        {
            _logger.LogWarning("Отдел с slug {Slug} не найден", slug);
            return Error.NotFound("directory.department.not_found", $"Отдел с slug {slug} не найден");
        }

        return department.Id.Value;
    }

    public async Task<Result<Domain.ValueObjects.Path, Error>> GetPathByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var department = await _dbContext.Departments
            .Where(x => x.Id == new DepartmentId(id))
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
}