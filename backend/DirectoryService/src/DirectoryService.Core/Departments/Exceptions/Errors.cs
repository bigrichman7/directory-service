using Shared;

namespace DirectoryService.Core.Exceptions;

public static partial class Errors
{
    public static class DepartmentExceptions
    {
        public static Error NotFound(Guid id) =>
            Error.NotFound(
                code: "department.not_found",
                message: $"Отдел с идентификатором '{id}' не найден.");
        public static Error Conflict(string message, string? invalidField = null) =>
            Error.Conflict(
                code: "department.conflict",
                message: message,
                invalidField: invalidField);
        public static Error Failure(string message) =>
            Error.Failure(
                code: "department.failure",
                message: message);
    }
}
