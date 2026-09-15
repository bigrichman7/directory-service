using DirectoryService.Contracts.Department;
using FluentValidation;

namespace DirectoryService.Core.Departments;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Название отдела не может быть пустым")
            .MaximumLength(200).MinimumLength(2).WithMessage("Название отдела невалидно");

        RuleFor(x => x.Slug)
            .NotEmpty().WithMessage("Slug не может быть пустым")
            .MaximumLength(200).MinimumLength(2)
            .Matches("^[a-z0-9]+(?:-[a-z0-9]+)*$").WithMessage("Slug имеет невалидный формат");
    }
}