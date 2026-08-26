using DirectoryService.Contracts.Location;
using FluentValidation;

namespace DirectoryService.Core.Locations;

public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200).MinimumLength(2).WithMessage("Название локации невалидно");

        RuleFor(x => x.City).NotEmpty().MaximumLength(100).MinimumLength(1).WithMessage("Неверное название города");

        RuleFor(x => x.Street).NotEmpty().MaximumLength(100).MinimumLength(1).WithMessage("Неверное название улицы");

        RuleFor(x => x.House).NotEmpty().MaximumLength(100).MinimumLength(1).WithMessage("Неверный адрес дома");

        RuleFor(x => x.Apartment).NotEmpty().MaximumLength(100).MinimumLength(1).WithMessage("Неверный номер квартиры");
    }
}
