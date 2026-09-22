using Shared;
using System.Text.Json.Serialization;

namespace DirectoryService.Web.EndpointResults;

public record Envelope
{
    public object? Result { get; }

    public Error? Errors { get; }

    public bool IsError => Errors != null;

    public DateTime TimeGenerated { get; }

    [JsonConstructor]
    private Envelope(object? result, Error? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.UtcNow;
    }

    public static Envelope Ok(object? result) => new(result, errors: null);
    public static Envelope Error(Error? errors) => new(result:null, errors);
}

public record Envelope<T>
{
    public T? Result { get; }
    public Error? Errors { get; }
    public bool IsError => Errors != null;
    public DateTime TimeGenerated { get; }
    [JsonConstructor]
    private Envelope(T? result, Error? errors)
    {
        Result = result;
        Errors = errors;
        TimeGenerated = DateTime.UtcNow;
    }
    internal static Envelope<T> Ok(T? result) => new(result, errors: null);
    internal static Envelope<T> Error(Error? errors) => new(result: default, errors);
}
