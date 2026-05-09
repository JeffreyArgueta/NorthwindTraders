using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class CreateShipperDtoValidator : AbstractValidator<CreateShipperDto>
{
    public CreateShipperDtoValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(40);
        When(x => x.Phone != null, () => RuleFor(x => x.Phone).MaximumLength(24));
    }
}
