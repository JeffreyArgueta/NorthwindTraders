using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class CreateCustomerDtoValidator : AbstractValidator<CreateCustomerDto>
{
    public CreateCustomerDtoValidator()
    {
        RuleFor(x => x.CustomerID).NotEmpty().MaximumLength(5);
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(40);
        When(x => x.Phone != null, () => RuleFor(x => x.Phone).MaximumLength(24));
    }
}
