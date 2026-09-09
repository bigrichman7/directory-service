using CSharpFunctionalExtensions;
using DirectoryService.Contracts.Department;
using DirectoryService.Core.Locations;
using DirectoryService.Domain.Common.Errors;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using DirectoryService.Domain.ValueObjects;
using ErrorOr;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace DirectoryService.Core.Departments;

public class DepartmentsService : IDepartmentsService
{

    private readonly IDepartmentsRepository _departmentsRepository;
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateDepartmentDto> _validator;
    private readonly ILogger<DepartmentsService> _logger;

    public DepartmentsService(IDepartmentsRepository departmentsRepository, ILocationsRepository locationsRepository, IValidator<CreateDepartmentDto> validator, ILogger<DepartmentsService> logger)
    {
        _departmentsRepository = departmentsRepository;
        _locationsRepository = locationsRepository;
        _validator = validator;
        _logger = logger;
    }
    public async Task<Result<Guid, Error>> Create(CreateDepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        var validationResult = await DepartmentDtoValidator(departmentDto, cancellationToken);
        if (validationResult.IsFailure)
            return validationResult.Error;

        DepartmentId? parentId = null;
        Domain.ValueObjects.Path? parentPath = null;

        if (departmentDto.ParentId != null)
        {
            parentId = new DepartmentId(departmentDto.ParentId.Value);
            var pathResult = await GetParentPath(parentId, cancellationToken);
            if (pathResult.IsFailure)
            {
                return Error.Failure(pathResult.Error.Code, pathResult.Error.Description);
            }
            parentPath = pathResult.Value;
        }

        var departmentResult = Department.Create(
            departmentDto.Name,
            departmentDto.Slug,
            parentId,
            parentPath);

        if (departmentResult.IsFailure)
        {
            return Error.Failure(departmentResult.Error.Code, departmentResult.Error.Message);
        }

        var department = departmentResult.Value;

        var locationResult = await CreateDepartmentLocation(
            department.Id,
            departmentDto.LocationIds,
            isPrimary: !departmentDto.ParentId.HasValue,
            cancellationToken);

        if (locationResult.IsFailure)
        {
            return Error.Failure(locationResult.Error.Code, locationResult.Error.Message);
        }

        var location = locationResult.Value;

        await _departmentsRepository.AddDepartmentWithDepartmentLocationAsync(department, location, cancellationToken);

        return department.Id.Value;
    }

    public async Task<Result<DepartmentResponse, Error>> Update(UpdateDepartmentDto departmentDto, CancellationToken cancellationToken)
    {
        var existingDepartmentResult = await _departmentsRepository.GetByIdAsync(new DepartmentId(departmentDto.Id), cancellationToken);
        if (existingDepartmentResult.IsFailure)
        {
            _logger.LogError("Подразделение с Id {DepartmentId} не найдено.", departmentDto.Id);
            return Error.Failure("directoryservice.department.not_found", $"Подразделение с Id {departmentDto.Id} не найдено.");
        }
        var existingDepartment = existingDepartmentResult.Value;

        if (departmentDto.Name == null && departmentDto.Slug == null)
        {
            _logger.LogError("Нет полей для обновления подразделения с Id {DepartmentId}.", departmentDto.Id);
            return Error.Failure("directoryservice.department.no_fields_to_update", "Нет полей для обновления.");
        }

        if (departmentDto.Name != null)
        {
            var updateNameResult = existingDepartment.UpdateName(departmentDto.Name);
            if (updateNameResult.IsFailure)
            {
                _logger.LogError("Ошибка при обновлении имени подразделения с Id {DepartmentId}: {ErrorMessage}", departmentDto.Id, updateNameResult.Error.Message);
                return Error.Failure(updateNameResult.Error.Code, updateNameResult.Error.Message);
            }
        }

        if (departmentDto.Slug != null)
        {
            var updateSlugResult = existingDepartment.UpdateSlug(departmentDto.Slug);
            if (updateSlugResult.IsFailure)
            {
                _logger.LogError("Ошибка при обновлении slug подразделения с Id {DepartmentId}: {ErrorMessage}", departmentDto.Id, updateSlugResult.Error.Message);
                return Error.Failure(updateSlugResult.Error.Code, updateSlugResult.Error.Message);
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

    public async Task<Result<DepartmentLocationResponse, Error>> AddLocation(Guid departmentId, Guid locationId, bool isPrimary, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentsRepository.GetByIdAsync(new DepartmentId(departmentId), cancellationToken);
        if (departmentResult.IsFailure)
        {
            _logger.LogError("Подразделение с Id {DepartmentId} не найдено.", departmentId);
            return Error.Failure("directoryservice.department.not_found", $"Подразделение с Id {departmentId} не найдено.");
        }

        var locationResult = await _locationsRepository.GetByIdAsync(new LocationId(locationId), cancellationToken);
        if (locationResult.IsFailure)
        {
            _logger.LogError("Локация с Id {LocationId} не найдена.", locationId);
            return Error.Failure("directoryservice.location.not_found", $"Локация с Id {locationId} не найдена.");
        }

        var departmentLocationResult = DepartmentLocation.Create(
            new DepartmentId(departmentId),
            new LocationId(locationId),
            isPrimary: isPrimary);

        if (departmentLocationResult.IsFailure)
        {
            _logger.LogError("Ошибка при создании связи между подразделением с Id {DepartmentId} и локацией с Id {LocationId}: {ErrorMessage}", departmentId, locationId, departmentLocationResult.Error.Message);
            return Error.Failure(departmentLocationResult.Error.Code, departmentLocationResult.Error.Message);
        }

        var departmentLocation = departmentLocationResult.Value;

        var addLocationResult = await _departmentsRepository.AddDepartmentLocationAsync(departmentLocation, cancellationToken);
        if (addLocationResult.IsFailure)
        {
            _logger.LogError("Ошибка при добавлении связи между подразделением с Id {DepartmentId} и локацией с Id {LocationId}: {ErrorMessage}", departmentId, locationId, addLocationResult.Error.Description);
            return Error.Failure(addLocationResult.Error.Code, addLocationResult.Error.Description);
        }

        var departmentLocationResponse = new DepartmentLocationResponse(
            departmentLocation.Id.Value,
            departmentLocation.DepartmentId.Value,
            departmentLocation.LocationId.Value,
            departmentLocation.IsPrimary);

        return departmentLocationResponse;
    }

    public async Task<Result<DepartmentLocationResponse, Error>> RemoveLocation(Guid departmentId, Guid locationId, CancellationToken cancellationToken)
    {
        var departmentResult = await _departmentsRepository.GetByIdAsync(new DepartmentId(departmentId), cancellationToken);
        if (departmentResult.IsFailure)
        {
            _logger.LogError("Подразделение с Id {DepartmentId} не найдено.", departmentId);
            return Error.Failure("directoryservice.department.not_found", $"Подразделение с Id {departmentId} не найдено.");
        }

        var locationResult = await _locationsRepository.GetByIdAsync(new LocationId(locationId), cancellationToken);
        if (locationResult.IsFailure)
        {
            _logger.LogError("Локация с Id {LocationId} не найдена.", locationId);
            return Error.Failure("directoryservice.location.not_found", $"Локация с Id {locationId} не найдена.");
        }

        var removeLocationResult = await _departmentsRepository.RemoveDepartmentLocationAsync(new DepartmentId(departmentId), new LocationId(locationId), cancellationToken);
        if (removeLocationResult.IsFailure)
        {
            _logger.LogError("Ошибка при удалении связи между подразделением с Id {DepartmentId} и локацией с Id {LocationId}: {ErrorMessage}", departmentId, locationId, removeLocationResult.Error.Description);
            return Error.Failure(removeLocationResult.Error.Code, removeLocationResult.Error.Description);
        }

        var departmentLocationResponse = new DepartmentLocationResponse(
            removeLocationResult.Value.Id.Value,
            removeLocationResult.Value.DepartmentId.Value,
            removeLocationResult.Value.LocationId.Value,
            removeLocationResult.Value.IsPrimary);

        return departmentLocationResponse;
    }

    private async Task<Result<CreateDepartmentDto, Error>> DepartmentDtoValidator(CreateDepartmentDto dto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
        {
            _logger.LogError("Ошибка валидации: {Errors}", validationResult.Errors);
            return Error.Failure("directoryservice.department.validation_failed", "Ошибка валидации CreateDepartmentDto.");
        }

        var nameExists = await _departmentsRepository.GetByNameAsync(Name.Create(dto.Name).Value, cancellationToken);
        if (nameExists.IsSuccess)
        {
            _logger.LogError("Подразделение с именем {Name} уже существует.", dto.Name);
            return Error.Failure("directoryservice.department.name_already_exists", $"Подразделение с именем {dto.Name} уже существует.");
        }

        var slugExists = await _departmentsRepository.GetBySlugAsync(Slug.Create(dto.Slug).Value, cancellationToken);
        if (slugExists.IsSuccess)
        {
            _logger.LogError("Подразделение с slug {Slug} уже существует.", dto.Slug);
            return Error.Failure("directoryservice.department.slug_already_exists", $"Подразделение с slug {dto.Slug} уже существует.");
        }

        return dto;
    }
    private async Task<Result<Domain.ValueObjects.Path, Error>> GetParentPath(DepartmentId parentId, CancellationToken cancellation)
    {
        var parentResult = await _departmentsRepository.GetByIdAsync(new DepartmentId(parentId.Value), cancellation);
        if (parentResult.IsFailure)
        {
            _logger.LogError("Родительское подразделение с Id {ParentId} не найдено.", parentId.Value);
            return Error.Failure("directoryservice.department.parent_not_found", $"Родительское подразделение с Id {parentId} не найдено.");
        }

        var pathResult = await _departmentsRepository.GetPathByIdAsync(new DepartmentId(parentId.Value), cancellation);
        if (pathResult.IsFailure)
        {
            _logger.LogError("Путь к родительскому подразделению с {ParentId} не найден", parentId.Value);
            return Error.Failure("directoryservice.department.path_not_found", $"Путь к родительскому подразделению с {parentId} не найден");
        }

        return pathResult.Value;
    }
    private async Task<Result<IEnumerable<DepartmentLocation>, DomainError>> CreateDepartmentLocation(DepartmentId departmentId, IEnumerable<Guid>? locationIds, bool isPrimary, CancellationToken cancellationToken)
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
            return GeneralErrors.ValueIsInvalid(locationResult.Error.Description);

        var foundIds = locationResult.Value.Select(l => l.Id.Value).ToHashSet();
        var missingIds = idsList.Where(id => !foundIds.Contains(id)).ToList();
        if (missingIds.Count > 0)
            return GeneralErrors.ValueIsInvalid($"Локации с ID {string.Join(", ", missingIds)} не найдены.");

        if (locationResult.IsFailure)
        {
            return GeneralErrors.ValueIsInvalid($"Локация с ID {locationIds} не найдены.");
        }

        foreach (var locationId in locationIds)
        {
            var departmentLocationResult = DepartmentLocation.Create(
                new DepartmentId(departmentId.Value),
                new LocationId(locationId),
                isPrimary);

            if (departmentLocationResult.IsFailure)
            {
                return GeneralErrors.ValueIsInvalid($"Не удалось создать связь между подразделением с ID {departmentId.Value} и локацией с ID {locationId}.");
            }

            departmentLocations.Add(departmentLocationResult.Value);
        }

        return departmentLocations;
    }
}
