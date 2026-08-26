using DirectoryService.Contracts.Location;
using FluentValidation;
using Microsoft.Extensions.Logging;
using DirectoryService.Domain.Locations;

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

        if (await _locationsRepository.GetByNameAsync(locationDto.Name, cancellationToken) != Guid.Empty)
        {
            throw new Exception("Такое имя локации уже существует.");
        }

        var locationId = Guid.CreateVersion7();
        var location = Location.Create(
            locationId,
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

        _logger.LogInformation("Location created with id {LocationId}", locationId);

        return locationId;
    }
}
