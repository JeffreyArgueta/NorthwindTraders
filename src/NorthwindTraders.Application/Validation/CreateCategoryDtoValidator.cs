using FluentValidation;
using NorthwindTraders.Domain.Contracts;

namespace NorthwindTraders.Application.Validation;

public class CreateCategoryDtoValidator : AbstractValidator<CategoryDto>
{
    public CreateCategoryDtoValidator()
    {
        RuleFor(x => x.CategoryID).GreaterThan(0);
        RuleFor(x => x.CategoryName).NotEmpty().MaximumLength(15);
    }
}
