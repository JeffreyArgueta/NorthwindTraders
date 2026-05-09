using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class CreateSupplierDtoValidator : AbstractValidator<CreateSupplierDto>
{
    public CreateSupplierDtoValidator()
    {
        RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200);
        When(x => x.Phone != null, () => RuleFor(x => x.Phone).MaximumLength(50));
    }
}
