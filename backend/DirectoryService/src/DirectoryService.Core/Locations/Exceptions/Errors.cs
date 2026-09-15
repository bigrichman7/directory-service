using Shared;

namespace DirectoryService.Core.Exceptions;

public static partial class Errors
{
    public static class LocationExceptions
    {
        public static Error NotFound(Guid id) =>
            Error.NotFound(
                code: "location.not.found",
                message: $"Локация с идентификатором '{id}' не найдена.");
        public static Error NotFound(IEnumerable<Guid> ids) =>
            Error.NotFound(
                code: "location.not.found",
                message: $"Локации с идентификаторами '{string.Join(", ", ids)}' не найдены.");
        public static Error Conflict(string message, string? invalidField = null) =>
            Error.Conflict(
                code: "location.conflict",
                message: message,
                invalidField: invalidField);
        public static Error Failure(string message) =>
            Error.Failure(
                code: "location.failure",
                message: message);
    }
}
