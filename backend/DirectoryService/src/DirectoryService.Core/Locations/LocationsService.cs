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
            throw new LocationBadRequestException(validationResult.ToErrors());
        }

        var checkName = await _locationsRepository.GetByNameAsync(locationDto.Name, cancellationToken);
        if (checkName != null)
        {
            throw new LocationConflictException(Errors.LocationExceptions.Conflict($"Локация с именем {locationDto.Name} уже существует.", "Name"));
        }

        var location = Location.Create(
            locationDto.Name,
            locationDto.City,
            locationDto.Street,
            locationDto.House,
            locationDto.Apartment);

        if (location.IsFailure)
        {
            throw new LocationBadRequestException(location.Error);
        }
        
        await _locationsRepository.AddAsync(location.Value, cancellationToken);

        _logger.LogInformation("Location created with id {LocationId}", location.Value.Id.Value);

        return location.Value.Id.Value;
    }

    public async Task<LocationResponse> Update(Guid locationId, UpdateLocationDto locationDto, CancellationToken cancellationToken)
    {
        if (locationDto.Name == null && locationDto.City == null && locationDto.Street == null && locationDto.House == null && locationDto.Apartment == null)
        {
            _logger.LogWarning("Нет данных для обновления локации с id {LocationId}", locationId);
            throw new LocationBadRequestException(Errors.Validations.InvalidData("Нет данных для обновления"));
        }

        var existingLocationResult = await _locationsRepository.GetByIdAsync(new LocationId(locationId), cancellationToken);
        if (existingLocationResult == null)
        {
            _logger.LogError("Локация с id {LocationId} не найдена", locationId);
            throw new LocationNotFoundException(Errors.LocationExceptions.NotFound(locationId));
        }

        var existingLocation = existingLocationResult;

        if (locationDto.Name is not null)
        {
            var updateNameResult = existingLocation.UpdateName(locationDto.Name);
            if (updateNameResult.IsFailure)
            {
                _logger.LogError("Ошибка при обновлении имени локации с id {LocationId}: {Error}", locationId, updateNameResult.Error);
                throw new LocationBadRequestException(updateNameResult.Error);
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
            throw new LocationBadRequestException(updateAddressResult.Error);
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
