using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shared.ResponseExtensions;

public static class ResponseExtension
{
    public static IActionResult ToResponse(this Failure failure)
    {
        if(!failure.Any())
        {
            return new ObjectResult(null)
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        var distinctErrorTypes = failure
            .Select(e => e.Type)
            .Distinct()
            .ToList();

        int statusCode = distinctErrorTypes.Count > 1
            ? StatusCodes.Status500InternalServerError
            : GetStatusCodeFromErrorType(distinctErrorTypes[0]);

        return new ObjectResult(failure)
        {
            StatusCode = statusCode,
        };
    }

    private static int GetStatusCodeFromErrorType(ErrorType errorType) =>
        errorType switch
        {
            ErrorType.NOT_FOUND => StatusCodes.Status404NotFound,
            ErrorType.VALIDATION => StatusCodes.Status400BadRequest,
            ErrorType.CONFLICT => StatusCodes.Status409Conflict,
            ErrorType.FAILURE => StatusCodes.Status500InternalServerError,
            _ => StatusCodes.Status500InternalServerError
        };

}
