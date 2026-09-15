using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Department;
using DirectoryService.Core.Exceptions;
using DirectoryService.Core.Departments.Exceptions;
using DirectoryService.Core.Locations.Exceptions;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Extensions;

namespace DirectoryService.Core.Departments;

public class DepartmentsService(
    IDepartmentsRepository departmentsRepository,
    ILocationsRepository locationsRepository,
    IValidator<CreateDepartmentDto> validator,
    ILogger<DepartmentsService> logger) : IDepartmentsService
{

    private readonly IDepartmentsRepository _departmentsRepository = departmentsRepository;
    private readonly ILocationsRepository _locationsRepository = locationsRepository;
    private readonly IValidator<CreateDepartmentDto> _validator = validator;
    private readonly ILogger<DepartmentsService> _logger = logger;

    public async Task<Guid> Create(CreateDepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        await DepartmentDtoValidator(departmentDto, cancellationToken);

        DepartmentId? parentId = null;
        Domain.ValueObjects.Path? parentPath = null;

        if (departmentDto.ParentId != null)
        {
            parentId = new DepartmentId(departmentDto.ParentId.Value);
            var parentDepartmentResult = await _departmentsRepository.GetByIdAsync(parentId, cancellationToken);
            if (parentDepartmentResult == null)
            {
                _logger.LogError("Родительское подразделение с Id {ParentId} не найдено", departmentDto.ParentId);
                throw new DepartmentNotFoundException(Errors.DepartmentExceptions.NotFound(departmentDto.ParentId.Value));
            }

            parentPath = parentDepartmentResult.Path;
        }

        var departmentResult = Department.Create(
            departmentDto.Name,
            departmentDto.Slug,
            parentId,
            parentPath);

        if (departmentResult.IsFailure)
        {
            throw new DepartmentBadRequestException(departmentResult.Error);
        }

        var department = departmentResult.Value;

        var departmentLocations = await CreateDepartmentLocation(
            department.Id,
            departmentDto.LocationIds,
            isPrimary: !departmentDto.ParentId.HasValue,
            cancellationToken);

        await _departmentsRepository.AddDepartmentWithDepartmentLocationAsync(department, departmentLocations, cancellationToken);

        return department.Id.Value;
    }

    public async Task<DepartmentResponse> Update(Guid departmentId, UpdateDepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        var existingDepartmentResult = await _departmentsRepository.GetByIdAsync(new DepartmentId(departmentId), cancellationToken);
        if (existingDepartmentResult == null)
        {
            _logger.LogError("Подразделение с Id {DepartmentId} не найдено.", departmentId);
            throw new DepartmentNotFoundException(Errors.DepartmentExceptions.NotFound(departmentId));
        }
        var existingDepartment = existingDepartmentResult;

        if (departmentDto.Name == null && departmentDto.Slug == null)
        {
            _logger.LogError("Нет полей для обновления подразделения с Id {DepartmentId}.", departmentId);
            throw new DepartmentBadRequestException(Errors.General.Failure("Нет полей для обновления."));
        }

        if (departmentDto.Name != null)
        {
            var updateNameResult = existingDepartment.UpdateName(departmentDto.Name);
            if (updateNameResult.IsFailure)
            {
                _logger.LogError("Ошибка при обновлении имени подразделения с Id {DepartmentId}: {ErrorMessage}", departmentId, updateNameResult.Error);
                throw new DepartmentBadRequestException(updateNameResult.Error);
            }
        }

        if (departmentDto.Slug != null)
        {
            var updateSlugResult = existingDepartment.UpdateSlug(departmentDto.Slug);
            if (updateSlugResult.IsFailure)
            {
                _logger.LogError("Ошибка при обновлении slug подразделения с Id {DepartmentId}: {ErrorMessage}", departmentId, updateSlugResult.Error);
                throw new DepartmentBadRequestException(updateSlugResult.Error);
            }
        }

        await  _departmentsRepository.UpdateAsync(existingDepartment, cancellationToken);

        var departmentResponse = new DepartmentResponse(
            existingDepartment.Id.Value,
            existingDepartment.ParentId?.Value ?? null,
            existingDepartment.Name.Value,
            existingDepartment.Slug.Value,
            existingDepartment.Path?.Value ?? null,
            existingDepartment.CreatedAt,
            existingDepartment.UpdatedAt);


        return departmentResponse;
    }

    public async Task<DepartmentLocationResponse> AddLocation(Guid departmentId, Guid locationId, bool isPrimary, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentsRepository.GetByIdAsync(new DepartmentId(departmentId), cancellationToken);
        if (departmentResult == null)
        {
            _logger.LogError("Подразделение с Id {DepartmentId} не найдено.", departmentId);
            throw new DepartmentNotFoundException(Errors.DepartmentExceptions.NotFound(departmentId));
        }

        var locationResult = await _locationsRepository.GetByIdAsync(new LocationId(locationId), cancellationToken);
        if (locationResult == null)
        {
            _logger.LogError("Локация с Id {LocationId} не найдена.", locationId);
            throw new LocationNotFoundException(Errors.LocationExceptions.NotFound(locationId));
        }

        var departmentLocationResult = DepartmentLocation.Create(
            new DepartmentId(departmentId),
            new LocationId(locationId),
            isPrimary: isPrimary);

        if (departmentLocationResult.IsFailure)
        {
            _logger.LogError(
                "Ошибка при создании связи между подразделением с Id {DepartmentId} и локацией с Id {LocationId}:",
                departmentId,
                locationId);
            throw new DepartmentBadRequestException(Errors.General.Failure("Не удалось создать связь между подразделением и локацией."));
        }

        var departmentLocation = departmentLocationResult.Value;

        var addLocationResult = await _departmentsRepository.AddDepartmentLocationAsync(departmentLocation, cancellationToken);
        if (addLocationResult == null)
        {
            _logger.LogError(
                "Ошибка при добавлении связи между подразделением с Id {DepartmentId} и локацией с Id {LocationId}",
                departmentId,
                locationId);

            throw new DepartmentConflictException(Errors.DepartmentExceptions.Conflict($"Связь отдела с Id {departmentId} и локации с Id {locationId} уже существует."));
        }

        var departmentLocationResponse = new DepartmentLocationResponse(
            departmentLocation.Id.Value,
            departmentLocation.DepartmentId.Value,
            departmentLocation.LocationId.Value,
            departmentLocation.IsPrimary);

        return departmentLocationResponse;
    }

    public async Task<DepartmentLocationResponse> RemoveLocation(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentsRepository.GetByIdAsync(new DepartmentId(departmentId), cancellationToken);
        if (departmentResult == null)
        {
            _logger.LogError("Подразделение с Id {DepartmentId} не найдено.", departmentId);
            throw new DepartmentNotFoundException(Errors.DepartmentExceptions.NotFound(departmentId));
        }

        var locationResult = await _locationsRepository.GetByIdAsync(new LocationId(locationId), cancellationToken);
        if (locationResult == null)
        {
            _logger.LogError("Локация с Id {LocationId} не найдена.", locationId);
            throw new LocationNotFoundException(Errors.LocationExceptions.NotFound(locationId));
        }

        var removeLocationResult = await _departmentsRepository.RemoveDepartmentLocationAsync(new DepartmentId(departmentId), new LocationId(locationId), cancellationToken);
        if (removeLocationResult == null)
        {
            _logger.LogError(
                "Ошибка при удалении связи между подразделением с Id {DepartmentId} и локацией с Id {LocationId}",
                departmentId,
                locationId);

            throw new DepartmentNotFoundException(Errors.DepartmentExceptions.NotFound($"Связь отдела с Id {departmentId} и локации с Id {locationId} не существует."));
        }

        var departmentLocationResponse = new DepartmentLocationResponse(
            removeLocationResult.Id.Value,
            removeLocationResult.DepartmentId.Value,
            removeLocationResult.LocationId.Value,
            removeLocationResult.IsPrimary);

        return departmentLocationResponse;
    }

    private async Task<CreateDepartmentDto> DepartmentDtoValidator(CreateDepartmentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Ошибка валидации: {Errors}", validationResult.ToErrors());
            throw new DepartmentBadRequestException(validationResult.ToErrors());
        }

        var nameExists = await _departmentsRepository.GetByNameAsync(Name.Create(dto.Name).Value, cancellationToken);
        if (nameExists != null)
        {
            _logger.LogError("Подразделение с именем {Name} уже существует.", dto.Name);
            throw new DepartmentConflictException(Errors.DepartmentExceptions.Conflict($"Подразделение с именем {dto.Name} уже существует.", "Name"));
        }

        var slugExists = await _departmentsRepository.GetBySlugAsync(Slug.Create(dto.Slug).Value, cancellationToken);
        if (slugExists != null)
        {
            _logger.LogError("Подразделение с slug {Slug} уже существует.", dto.Slug);
            throw new DepartmentConflictException(Errors.DepartmentExceptions.Conflict($"Подразделение с slug {dto.Slug} уже существует.", "Slug"));
        }

        return dto;
    }
    private async Task<IEnumerable<DepartmentLocation>> CreateDepartmentLocation(DepartmentId departmentId, IEnumerable<Guid>? locationIds, bool isPrimary, CancellationToken cancellationToken)
    {
        var departmentLocations = new List<DepartmentLocation>();

        if (locationIds == null)
        {
            return departmentLocations;
        }

        List<Guid> idsList = (List<Guid>)locationIds;
        if (idsList.Count == 0)
            return departmentLocations;

        var locationResult = await _locationsRepository.GetByIdsAsync(locationIds, cancellationToken);

        var foundIds = locationResult.Select(l => l.Id.Value).ToHashSet();
        var missingIds = idsList.Where(id => !foundIds.Contains(id)).ToList();
        if (missingIds.Count > 0)
            throw new LocationNotFoundException(Errors.LocationExceptions.NotFound(missingIds));

        foreach (var locationId in locationIds)
        {
            var departmentLocationResult = DepartmentLocation.Create(
                new DepartmentId(departmentId.Value),
                new LocationId(locationId),
                isPrimary);

            if (departmentLocationResult.IsFailure)
            {
                throw new DepartmentBadRequestException(Errors.DepartmentExceptions.Failure($"Не удалось создать связь между подразделением с ID {departmentId.Value} и локацией с ID {locationId}."));
            }

            departmentLocations.Add(departmentLocationResult.Value);
        }

        return departmentLocations;
    }
}
