using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeDtoValidator()
    {
        When(x => x.LastName != null, () => RuleFor(x => x.LastName).NotEmpty().MaximumLength(20));
        When(x => x.FirstName != null, () => RuleFor(x => x.FirstName).NotEmpty().MaximumLength(10));
    }
}
