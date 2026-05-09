using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class UpdateProductDtoValidator : AbstractValidator<UpdateProductDto>
{
    public UpdateProductDtoValidator()
    {
        When(x => x.ProductName != null, () => RuleFor(x => x.ProductName).NotEmpty().MaximumLength(200));
        When(x => x.UnitPrice != null, () => RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0));
    }
}
