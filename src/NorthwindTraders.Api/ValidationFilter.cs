using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace NorthwindTraders.Api;

// Minimal endpoint validation helper to be used from handlers
public sealed class ValidationFilter
{
    private readonly IServiceProvider _sp;

    public ValidationFilter(IServiceProvider sp)
    {
        _sp = sp;
    }

    public async Task<IResult?> ValidateAsync<T>(T instance)
    {
        var validator = _sp.GetService(typeof(IValidator<T>)) as IValidator<T>;
        if (validator == null) return null; // no validator - allow
        var result = await validator.ValidateAsync(instance);
        if (result.IsValid) return null;
        var errors = result.Errors.Select(e => new { e.PropertyName, e.ErrorMessage }).ToArray();
        return Results.BadRequest(new { Errors = errors });
    }
}
