using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class UpdateSupplierDtoValidator : AbstractValidator<UpdateSupplierDto>
{
    public UpdateSupplierDtoValidator()
    {
        When(x => x.CompanyName != null, () => RuleFor(x => x.CompanyName).NotEmpty().MaximumLength(200));
    }
}
