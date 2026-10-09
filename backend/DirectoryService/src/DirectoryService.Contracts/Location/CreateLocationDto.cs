namespace DirectoryService.Contracts.Location;

public record CreateLocationDto(string Name, AddressDto Address);

public record AddressDto(
    string City,
    string Street,
    string House,
    string Apartment);
