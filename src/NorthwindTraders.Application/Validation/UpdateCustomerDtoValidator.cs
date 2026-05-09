using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class UpdateCustomerDtoValidator : AbstractValidator<UpdateCustomerDto>
{
    public UpdateCustomerDtoValidator()
    {
        When(x => x.CompanyName != null, () => RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(40));
        When(x => x.Phone != null, () => RuleFor(x => x.Phone).MaximumLength(24));
    }
}
