using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Contracts.Location;

public record CreateLocationDto(string Name, string City, string Street, string House, string Apartment);
