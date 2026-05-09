using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class CreateEmployeeTerritoryDtoValidator : AbstractValidator<EmployeeTerritoryDto>
{
    public CreateEmployeeTerritoryDtoValidator()
    {
        RuleFor(x => x.EmployeeID).GreaterThan(0);
        RuleFor(x => x.TerritoryID).NotEmpty();
    }
}
