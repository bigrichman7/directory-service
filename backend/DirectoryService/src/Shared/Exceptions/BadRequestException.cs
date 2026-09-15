using System.Text.Json;

namespace Shared.Exceptions;

public class BadRequestException : Exception
{
    protected BadRequestException(Error[] errors)
        : base(JsonSerializer.Serialize(errors))
    {
    }

    private BadRequestException()
    {
    }

    private BadRequestException(string message) : base(message)
    {
    }

    private BadRequestException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
