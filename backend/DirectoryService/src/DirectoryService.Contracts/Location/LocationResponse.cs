using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Contracts.Location;
public record LocationResponse(
    Guid Id,
    string Name,
    string Address,
    DateTime CreatedAt,
    DateTime UpdatedAt
);
