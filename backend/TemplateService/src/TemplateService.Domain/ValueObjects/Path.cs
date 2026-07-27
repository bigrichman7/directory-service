
using CSharpFunctionalExtensions;

namespace TemplateService.Domain.ValueObjects;

public record Path
{
    public const string Separator = "/";
    private Path(string value) => Value = value;

    public string Value { get; private set; }

    public static Result<Path> Create(Guid? parentId, Slug slug)
    {
        if (parentId == null)
            return new Path($"{Separator}{slug.Value}");
        
        return new Path($"pathParentId{Separator}{slug.Value}");
    }

    public static implicit operator string(Path value) => value.Value;
    public override string ToString() => Value;
}

