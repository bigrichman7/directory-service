using System.Text.Json;

namespace Shared.Exceptions;

public class NotFoundException : Exception
{
    protected NotFoundException(Error[] errors)
        : base(JsonSerializer.Serialize(errors))
    {
    }

    private NotFoundException()
    {
    }

    private NotFoundException(string message) : base(message)
    {
    }

    private NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
