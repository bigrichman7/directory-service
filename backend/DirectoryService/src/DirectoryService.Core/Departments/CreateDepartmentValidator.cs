using DirectoryService.Contracts.Department;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace DirectoryService.Core.Departments;

public class CreateDepartmentValidator : AbstractValidator<CreateDepartmentDto>
{
    public CreateDepartmentValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200).MinimumLength(2).WithMessage("Название отдела невалидно");
        RuleFor(x => x.Slug).NotEmpty().MaximumLength(200).MinimumLength(2).WithMessage("Slug не может быть пустым");
    }
}
