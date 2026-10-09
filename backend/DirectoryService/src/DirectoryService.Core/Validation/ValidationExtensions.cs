using Shared;
using FluentValidation.Results;
using System.Text.Json;

namespace DirectoryService.Core.Validation;

public static class ValidationExtensions
{
    public static Error ToError(this ValidationResult validationResult)
    {
        List<ValidationFailure> validationErrors = validationResult.Errors;

        List<ErrorMessages> errors = validationErrors
            .Select(ve => ve.ErrorMessage)
            .SelectMany(msg => JsonSerializer.Deserialize<List<ErrorMessages>>(msg) ?? [])
            .ToList();

        return Error.Validation([..errors]);
    }
}
