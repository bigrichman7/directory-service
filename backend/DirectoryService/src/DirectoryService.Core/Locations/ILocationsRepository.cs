using DirectoryService.Domain.Locations;
using DirectoryService.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Core.Locations;

public interface ILocationsRepository
{
    Task<Guid> AddAsync(Location location, CancellationToken cancellationToken);

    Task<Guid> GetByNameAsync(string name, CancellationToken cancellationToken);
}
