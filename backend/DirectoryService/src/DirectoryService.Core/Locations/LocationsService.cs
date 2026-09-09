using DirectoryService.Contracts.Location;
using FluentValidation;
using Microsoft.Extensions.Logging;
using DirectoryService.Domain.Locations;
using ErrorOr;
using CSharpFunctionalExtensions;

namespace DirectoryService.Core.Locations;

public class LocationsService : ILocationsService
{
    private readonly ILocationsRepository _locationsRepository;
    private readonly IValidator<CreateLocationDto> _validator;
    private readonly ILogger<LocationsService> _logger;
    public LocationsService(ILocationsRepository locationsRepository, IValidator<CreateLocationDto> validator, ILogger<LocationsService> logger)
    {
        _locationsRepository = locationsRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<Guid> Create(CreateLocationDto locationDto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(locationDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var checkName = await _locationsRepository.GetByNameAsync(locationDto.Name, cancellationToken);
        if (checkName.IsSuccess)
        {
            throw new Exception("Такое имя локации уже существует.");
        }

        var location = Location.Create(
            locationDto.Name,
            locationDto.City,
            locationDto.Street,
            locationDto.House,
            locationDto.Apartment);

        if (location.IsFailure)
        {
            throw new Exception(location.Error.ToString());
        }

        
        await _locationsRepository.AddAsync(location.Value, cancellationToken);

        _logger.LogInformation("Location created with id {LocationId}", location.Value.Id.Value);

        return location.Value.Id.Value;
    }

    public async Task<Result<Location, Error>> Update(UpdateLocationDto locationDto, CancellationToken cancellationToken)
    {
        if (locationDto.Name == null && locationDto.City == null && locationDto.Street == null && locationDto.House == null && locationDto.Apartment == null)
        {
            _logger.LogWarning("Нет данных для обновления локации с id {LocationId}", locationDto.Id);
            return Error.Failure("directory.location.update_no_data", $"Нет данных для обновления локации с id {locationDto.Id}");
        }

        var existingLocationResult = await _locationsRepository.GetByIdAsync(new LocationId(locationDto.Id), cancellationToken);
        if (existingLocationResult.IsFailure)
        {
            _logger.LogError("Локация с id {LocationId} не найдена", locationDto.Id);
            return Error.Failure("directory.location.not_found", $"Локация с id {locationDto.Id} не найдена");
        }

        var existingLocation = existingLocationResult.Value;

        if (locationDto.Name is not null)
        {
            var updateNameResult = existingLocation.UpdateName(locationDto.Name);
            if (updateNameResult.IsFailure)
            {
                _logger.LogError("Ошибка при обновлении имени локации с id {LocationId}: {Error}", locationDto.Id, updateNameResult.Error);
                return Error.Failure("directory.location.update_name_failed", $"Ошибка при обновлении имени локации с id {locationDto.Id}: {updateNameResult.Error}");
            }
        }

        var newCity = locationDto.City ?? existingLocation.Address.City;
        var newStreet = locationDto.Street ?? existingLocation.Address.Street;
        var newHouse = locationDto.House ?? existingLocation.Address.House;
        var newApartment = locationDto.Apartment ?? existingLocation.Address.Apartment;

        var updateAddressResult = existingLocation.UpdateAddress(newCity, newStreet, newHouse, newApartment);
        if (updateAddressResult.IsFailure)
        {
            _logger.LogError("Ошибка при обновлении адреса локации с id {LocationId}: {Error}", locationDto.Id, updateAddressResult.Error);
            return Error.Failure("directory.location.update_address_failed", $"Ошибка при обновлении адреса локации с id {locationDto.Id}: {updateAddressResult.Error}");
        }

        await _locationsRepository.UpdateAsync(existingLocation, cancellationToken);

        return existingLocation;
    }
}
