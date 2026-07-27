namespace DirectoryService.Domain.Common.Errors;

public static class GeneralErrors
{
    public static DomainError ValueIsInvalid(string v) =>
        new("GeneralErrors.ValueIsInvalid", $"Значение недействительно: {v}", ErrorType.Validation);

    public static DomainError ValueIsRequired(string v) =>
        new("GeneralErrors.ValueIsRequired", $"Отсутствует значение: {v}", ErrorType.Required);
}