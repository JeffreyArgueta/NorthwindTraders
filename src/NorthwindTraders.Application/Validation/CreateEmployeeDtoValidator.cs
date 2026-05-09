using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class CreateEmployeeDtoValidator : AbstractValidator<CreateEmployeeDto>
{
    public CreateEmployeeDtoValidator()
    {
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(20);
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(10);
    }
}
