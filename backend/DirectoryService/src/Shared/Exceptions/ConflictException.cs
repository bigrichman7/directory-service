using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Shared.Exceptions;

public class ConflictException : Exception
{
    public ConflictException(Error[] errors)
        : base(JsonSerializer.Serialize(errors))
    {
    }

    private ConflictException() : base()
    {
    }

    private ConflictException(string? message) : base(message)
    {
    }

    private ConflictException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
