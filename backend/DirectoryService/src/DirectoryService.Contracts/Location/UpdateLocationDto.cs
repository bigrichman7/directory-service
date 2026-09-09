namespace DirectoryService.Contracts.Location;

public record UpdateLocationDto(Guid Id, string? Name, string? City, string? Street, string? House, string? Apartment);
