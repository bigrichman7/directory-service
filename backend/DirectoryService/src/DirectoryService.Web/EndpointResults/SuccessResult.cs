using System.Net;

namespace DirectoryService.Web.EndpointResults;

public sealed class SuccessResult<TValue> : IResult
{
    private readonly TValue _value;

    private readonly int _statusCode;

    public SuccessResult(TValue value, int statusCode = (int)HttpStatusCode.OK)
	{
        _value = value;
        _statusCode = statusCode;
	}

    public Task ExecuteAsync(HttpContext httpContext)
    {
        ArgumentNullException.ThrowIfNull(httpContext);

        var envelope = Envelope.Ok(_value);

        httpContext.Response.StatusCode = _statusCode;

        return httpContext.Response.WriteAsJsonAsync(envelope);
    }
}
