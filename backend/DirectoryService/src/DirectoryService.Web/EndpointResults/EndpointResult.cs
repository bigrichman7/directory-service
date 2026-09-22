using CSharpFunctionalExtensions;
using Shared;
using System.Net;

namespace DirectoryService.Web.EndpointResults;

public sealed class EndpointResult<TValue> : Microsoft.AspNetCore.Http.IResult
{
    private readonly Microsoft.AspNetCore.Http.IResult _result;

    public EndpointResult(Result<TValue, Error> result, int statusCode = (int)HttpStatusCode.OK)
    {
        _result = result.IsSuccess 
            ? new SuccessResult<TValue>(result.Value, statusCode)
            : new ErrorResult(result.Error);
    }


    public Task ExecuteAsync(HttpContext httpContext) =>
        _result.ExecuteAsync(httpContext);


    public static implicit operator EndpointResult<TValue>(Result<TValue, Error> result) => new(result);
}
