using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class UpdateShipperDtoValidator : AbstractValidator<UpdateShipperDto>
{
    public UpdateShipperDtoValidator()
    {
        When(x => x.CompanyName != null, () => RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(40));
    }
}
