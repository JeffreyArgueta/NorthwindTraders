using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Builder;

namespace NorthwindTraders.Api;

// Generic endpoint filter that runs a FluentValidation validator for the DTO type T
public sealed class ValidatorEndpointFilter<T> : IEndpointFilter
{
    private readonly IValidator<T>? _validator;

    public ValidatorEndpointFilter(IValidator<T>? validator)
    {
        _validator = validator;
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        if (_validator != null)
        {
            // find first argument of type T
            var arg = context.Arguments.FirstOrDefault(a => a is T);
            if (arg is T instance)
            {
                var result = await _validator.ValidateAsync(instance!);
                if (!result.IsValid)
                {
                    var errors = result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToArray();
                    return Results.BadRequest(new { Errors = errors });
                }
            }
        }

        return await next(context);
    }
}
