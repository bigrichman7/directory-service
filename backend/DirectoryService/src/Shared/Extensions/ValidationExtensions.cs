using FluentValidation.Results;

namespace Shared.Extensions;

public static class ValidationExtensions
{
    public static Error ToErrors(this ValidationResult validationResult) =>
        Error.Validation([.. validationResult.Errors.Select(e => new ErrorMessages(e.ErrorCode, e.ErrorMessage, e.PropertyName))]);
}
