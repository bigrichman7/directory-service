using DirectoryService.Contracts.Location;
using FluentValidation;
using Microsoft.Extensions.Logging;
using DirectoryService.Domain.Locations;
using CSharpFunctionalExtensions;
using Shared;
using DirectoryService.Core.Exceptions;
using DirectoryService.Core.Locations.Exceptions;
using Shared.Extensions;

namespace DirectoryService.Core.Locations;

public class LocationsService(
    ILocationsRepository locationsRepository,
    IValidator<CreateLocationDto> validator,
    ILogger<LocationsService> logger) : ILocationsService
{
    private readonly ILocationsRepository _locationsRepository = locationsRepository;
    private readonly IValidator<CreateLocationDto> _validator = validator;
    private readonly ILogger<LocationsService> _logger = logger;

    public async Task<Result<Guid, Error>> Create(CreateLocationDto locationDto, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(locationDto, cancellationToken);
        if (!validationResult.IsValid)
        {
            return validationResult.ToErrors();
        }

        var checkName = await _locationsRepository.GetByNameAsync(locationDto.Name, cancellationToken);
        if (checkName.IsSuccess)
        {
            return Errors.LocationExceptions.Conflict($"Локация с именем {locationDto.Name} уже существует.", "Name");
        }

        var location = Location.Create(
            locationDto.Name,
            locationDto.City,
            locationDto.Street,
            locationDto.House,
            locationDto.Apartment);

        if (location.IsFailure)
        {
            return location.Error;
        }
        
        await _locationsRepository.AddAsync(location.Value, cancellationToken);

        var newLocationId = location.Value.Id.Value;
        _logger.LogInformation("Локация с Id {NewLocationId} создана", newLocationId);

        return newLocationId;
    }

    public async Task<Result<LocationResponse, Error>> Update(Guid locationId, UpdateLocationDto locationDto, CancellationToken cancellationToken)
    {
        if (locationDto.Name == null && locationDto.City == null && locationDto.Street == null && locationDto.House == null && locationDto.Apartment == null)
        {
            _logger.LogWarning("Нет данных для обновления локации с id {LocationId}", locationId);
            return Errors.Validations.InvalidData("Нет данных для обновления");
        }

        var existingLocationResult = await _locationsRepository.GetByIdAsync(new LocationId(locationId), cancellationToken);
        if (existingLocationResult.IsFailure)
        {
            _logger.LogError("Локация с id {LocationId} не найдена", locationId);
            return Errors.LocationExceptions.NotFound(locationId);
        }

        var existingLocation = existingLocationResult.Value;

        if (locationDto.Name is not null)
        {
            var updateNameResult = existingLocation.UpdateName(locationDto.Name);
            if (updateNameResult.IsFailure)
            {
                _logger.LogError("Ошибка при обновлении имени локации с id {LocationId}: {Error}", locationId, updateNameResult.Error);
                return updateNameResult.Error;
            }
        }

        var newCity = locationDto.City ?? existingLocation.Address.City;
        var newStreet = locationDto.Street ?? existingLocation.Address.Street;
        var newHouse = locationDto.House ?? existingLocation.Address.House;
        var newApartment = locationDto.Apartment ?? existingLocation.Address.Apartment;

        var updateAddressResult = existingLocation.UpdateAddress(newCity, newStreet, newHouse, newApartment);
        if (updateAddressResult.IsFailure)
        {
            _logger.LogError("Ошибка при обновлении адреса локации с id {LocationId}: {Error}", locationId, updateAddressResult.Error);
            return updateAddressResult.Error;
        }

        await _locationsRepository.UpdateAsync(existingLocation, cancellationToken);

        return new LocationResponse(
            existingLocation.Id.Value,
            existingLocation.Name,
            existingLocation.Address.ToString(),
            existingLocation.CreatedAt,
            existingLocation.UpdatedAt
        );
    }
}
