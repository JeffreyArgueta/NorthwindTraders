using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class UpdateOrderDetailDtoValidator : AbstractValidator<UpdateOrderDetailDto>
{
    public UpdateOrderDetailDtoValidator()
    {
        RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.Quantity).GreaterThan((short)0);
    }
}
