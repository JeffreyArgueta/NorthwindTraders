using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class UpdateOrderDtoValidator : AbstractValidator<UpdateOrderDto>
{
    public UpdateOrderDtoValidator()
    {
        When(x => x.Freight != default, () => RuleFor(x => x.Freight).GreaterThanOrEqualTo(0));
    }
}
