
using CSharpFunctionalExtensions;
using DirectoryService.Domain.Departments;

namespace DirectoryService.Domain.ValueObjects;

public record Path
{
    public const string Separator = "/";
    private Path(string value) => Value = value;

    public string Value { get; private set; }

    public static Result<Path> Create(Department? parentDepartment, Slug slug)
    {
        if (parentDepartment == null)
            return new Path($"{Separator}{slug.Value}");
        
        return new Path($"{parentDepartment.Path}{Separator}{slug.Value}");
    }

    public static implicit operator string(Path value) => value.Value;
    public override string ToString() => Value;
}

