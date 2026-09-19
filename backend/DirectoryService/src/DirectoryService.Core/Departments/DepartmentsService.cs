using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Department;
using DirectoryService.Core.Exceptions;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Shared.Extensions;
using Shared;

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

    public async Task<Result<Guid, Error>> Create(CreateDepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        var departmentDtoValidatorResult = await DepartmentDtoValidator(departmentDto, cancellationToken);
        if (departmentDtoValidatorResult.IsFailure)
        {
            return departmentDtoValidatorResult.Error;
        }

        DepartmentId? parentId = null;
        Domain.ValueObjects.Path? parentPath = null;

        if (departmentDto.ParentId != null)
        {
            parentId = new DepartmentId(departmentDto.ParentId.Value);
            var parentDepartmentResult = await _departmentsRepository.GetByIdAsync(parentId, cancellationToken);
            if (parentDepartmentResult.IsFailure)
            {
                _logger.LogError("Родительское подразделение с Id {ParentId} не найдено", departmentDto.ParentId);
                return Errors.DepartmentExceptions.NotFound(departmentDto.ParentId.Value);
            }

            parentPath = parentDepartmentResult.Value.Path;
        }

        var departmentResult = Department.Create(
            departmentDto.Name,
            departmentDto.Slug,
            parentId,
            parentPath);

        if (departmentResult.IsFailure)
        {
            return departmentResult.Error;
        }

        var department = departmentResult.Value;

        var departmentLocationsResult = await CreateDepartmentLocation(
            department.Id,
            departmentDto.LocationIds,
            isPrimary: !departmentDto.ParentId.HasValue,
            cancellationToken);

        if(departmentLocationsResult.IsFailure)
        {
            return departmentLocationsResult.Error;
        }

        await _departmentsRepository.AddDepartmentWithDepartmentLocationAsync(department, departmentLocationsResult.Value, cancellationToken);

        return department.Id.Value;
    }

    public async Task<Result<DepartmentResponse, Error>> Update(Guid departmentId, UpdateDepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        var existingDepartmentResult = await _departmentsRepository.GetByIdAsync(new DepartmentId(departmentId), cancellationToken);
        if (existingDepartmentResult.IsFailure)
        {
            _logger.LogError("Подразделение с Id {DepartmentId} не найдено.", departmentId);
            return Errors.DepartmentExceptions.NotFound(departmentId);
        }
        var existingDepartment = existingDepartmentResult.Value;

        if (departmentDto.Name == null && departmentDto.Slug == null && departmentDto.ParentId == null)
        {
            _logger.LogError("Нет полей для обновления подразделения с Id {DepartmentId}.", departmentId);
            return Errors.General.Failure("Нет полей для обновления.");
        }

        if (departmentDto.Name != null)
        {
            var updateNameResult = existingDepartment.UpdateName(departmentDto.Name);
            if (updateNameResult.IsFailure)
            {
                _logger.LogError("Ошибка при обновлении имени подразделения с Id {DepartmentId}: {ErrorMessage}", departmentId, updateNameResult.Error);
                return updateNameResult.Error;
            }
        }

        if (departmentDto.Slug != null)
        {
            var updateSlugResult = existingDepartment.UpdateSlug(departmentDto.Slug);
            if (updateSlugResult.IsFailure)
            {
                _logger.LogError("Ошибка при обновлении slug подразделения с Id {DepartmentId}: {ErrorMessage}", departmentId, updateSlugResult.Error);
                return updateSlugResult.Error;
            }
        }


        await  _departmentsRepository.UpdateAsync(existingDepartment, cancellationToken);

        return new DepartmentResponse(
            existingDepartment.Id.Value,
            existingDepartment.ParentId?.Value ?? null,
            existingDepartment.Name.Value,
            existingDepartment.Slug.Value,
            existingDepartment.Path?.Value ?? null,
            existingDepartment.CreatedAt,
            existingDepartment.UpdatedAt);
    }

    public async Task<Result<DepartmentLocationResponse, Error>> AddLocation(Guid departmentId, Guid locationId, bool isPrimary, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentsRepository.GetByIdAsync(new DepartmentId(departmentId), cancellationToken);
        if (departmentResult.IsFailure)
        {
            _logger.LogError("Подразделение с Id {DepartmentId} не найдено.", departmentId);
            return Errors.DepartmentExceptions.NotFound(departmentId);
        }

        var locationResult = await _locationsRepository.GetByIdAsync(new LocationId(locationId), cancellationToken);
        if (locationResult.IsFailure)
        {
            _logger.LogError("Локация с Id {LocationId} не найдена.", locationId);
            return Errors.LocationExceptions.NotFound(locationId);
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
            return Errors.DepartmentExceptions.Failure("Не удалось создать связь между подразделением и локацией.");
        }

        var departmentLocation = departmentLocationResult.Value;

        var addLocationResult = await _departmentsRepository.AddDepartmentLocationAsync(departmentLocation, cancellationToken);
        if (addLocationResult.IsFailure)
        {
            _logger.LogError(
                "Ошибка при добавлении связи между подразделением с Id {DepartmentId} и локацией с Id {LocationId}",
                departmentId,
                locationId);

            return Errors.DepartmentExceptions.Conflict($"Связь отдела с Id {departmentId} и локации с Id {locationId} уже существует.");
        }

        return new DepartmentLocationResponse(
            departmentLocation.Id.Value,
            departmentLocation.DepartmentId.Value,
            departmentLocation.LocationId.Value,
            departmentLocation.IsPrimary);
    }

    public async Task<Result<DepartmentLocationResponse, Error>> RemoveLocation(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentsRepository.GetByIdAsync(new DepartmentId(departmentId), cancellationToken);
        if (departmentResult.IsFailure)
        {
            _logger.LogError("Подразделение с Id {DepartmentId} не найдено.", departmentId);
            return Errors.General.NotFound(departmentId);
        }

        var locationResult = await _locationsRepository.GetByIdAsync(new LocationId(locationId), cancellationToken);
        if (locationResult.IsFailure)
        {
            _logger.LogError("Локация с Id {LocationId} не найдена.", locationId);
            return Errors.General.NotFound(locationId);
        }

        var removeLocationResult = await _departmentsRepository.RemoveDepartmentLocationAsync(new DepartmentId(departmentId), new LocationId(locationId), cancellationToken);
        if (removeLocationResult.IsFailure)
        {
            _logger.LogError(
                "Ошибка при удалении связи между подразделением с Id {DepartmentId} и локацией с Id {LocationId}",
                departmentId,
                locationId);

            return Errors.DepartmentExceptions.NotFound($"Связь отдела с Id {departmentId} и локации с Id {locationId} не существует.");
        }

        var removedLocation = removeLocationResult.Value;

        return new DepartmentLocationResponse(
            removedLocation.Id.Value,
            removedLocation.DepartmentId.Value,
            removedLocation.LocationId.Value,
            removedLocation.IsPrimary);
    }

    private async Task<Result<CreateDepartmentDto, Error>> DepartmentDtoValidator(CreateDepartmentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Ошибка валидации: {Errors}", validationResult.ToErrors());
            return validationResult.ToErrors();
        }

        var nameExists = await _departmentsRepository.GetByNameAsync(Name.Create(dto.Name).Value, cancellationToken);
        if (nameExists.IsSuccess)
        {
            _logger.LogError("Подразделение с именем {Name} уже существует.", dto.Name);
            return Errors.DepartmentExceptions.Conflict($"Подразделение с именем {dto.Name} уже существует.", "Name");
        }

        var slugExists = await _departmentsRepository.GetBySlugAsync(Slug.Create(dto.Slug).Value, cancellationToken);
        if (slugExists.IsSuccess)
        {
            _logger.LogError("Подразделение с slug {Slug} уже существует.", dto.Slug);
            return Errors.DepartmentExceptions.Conflict($"Подразделение с slug {dto.Slug} уже существует.", "Slug");
        }

        return dto;
    }
    private async Task<Result<IEnumerable<DepartmentLocation>, Error>> CreateDepartmentLocation(DepartmentId departmentId, IEnumerable<Guid>? locationIds, bool isPrimary, CancellationToken cancellationToken)
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
        if (locationResult.IsFailure)
        {
            _logger.LogError("Ошибка при получении локаций: {ErrorMessage}", locationResult.Error);
            return locationResult.Error;
        }

        var location = locationResult.Value;

        var foundIds = location.Select(l => l.Id.Value).ToHashSet();
        var missingIds = idsList.Where(id => !foundIds.Contains(id)).ToList();
        if (missingIds.Count > 0)
            return Errors.LocationExceptions.NotFound(missingIds);

        foreach (var locationId in locationIds)
        {
            var departmentLocationResult = DepartmentLocation.Create(
                new DepartmentId(departmentId.Value),
                new LocationId(locationId),
                isPrimary);

            if (departmentLocationResult.IsFailure)
            {
                return departmentLocationResult.Error;
            }

            departmentLocations.Add(departmentLocationResult.Value);
        }

        return departmentLocations;
    }
}
