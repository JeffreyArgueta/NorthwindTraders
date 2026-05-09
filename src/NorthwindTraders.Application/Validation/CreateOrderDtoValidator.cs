using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class CreateOrderDtoValidator : AbstractValidator<CreateOrderDto>
{
    public CreateOrderDtoValidator()
    {
        When(x => x.Freight != default, () => RuleFor(x => x.Freight).GreaterThanOrEqualTo(0));
        When(x => x.OrderDetails != null, () => RuleForEach(x => x.OrderDetails).SetValidator(new CreateOrderDetailDtoValidator()));
    }
}
