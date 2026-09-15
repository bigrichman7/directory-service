using Shared;

namespace DirectoryService.Core.Exceptions;

public static partial class Errors
{
    public static class General
    {
        public static Error NotFound(Guid id) =>
            Error.NotFound(
                code: "error.not.found",
                message: $"Запись с идентификатором '{id}' не найдена.");

        public static Error NotFound(IEnumerable<Guid> ids) =>
            Error.NotFound(
                code: "error.not.found",
                message: $"Записи с идентификаторами '{string.Join(", ", ids)}' не найдены.");

        public static Error Conflict(string message) =>
            Error.Conflict(
                code: "error.conflict",
                message: message);

        public static Error Failure(string message) =>
            Error.Failure(
                code: "error.failure",
                message: message);
    }

    public static class Validations
    {
        public static Error InvalidData(string data) =>
            Error.Failure(
                code: "error.invalid.data",
                message: $"Данные '{data}' являются недопустимыми.");
    }
}
