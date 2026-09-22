namespace DirectoryService.Contracts.Position;

public record PositionResponse(Guid Id, string Name, DateTime CreatedAt, DateTime UpdatedName);
