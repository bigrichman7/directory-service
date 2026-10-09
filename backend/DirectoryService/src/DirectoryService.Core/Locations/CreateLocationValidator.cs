using DirectoryService.Contracts.Location;
using DirectoryService.Core.Validation;
using DirectoryService.Domain.ValueObjects;
using FluentValidation;

namespace DirectoryService.Core.Locations;

public class CreateLocationValidator : AbstractValidator<CreateLocationDto>
{
    public CreateLocationValidator()
    {
        RuleFor(x => x.Name)
            .MustBeValueObject(Name.Create);

        RuleFor(x => x.Address)
            .MustBeValueObject(dto => Address.Create(dto.City, dto.Street, dto.House, dto.Apartment));
    }
}
