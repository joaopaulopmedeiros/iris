using FluentValidation.Results;

namespace Iris.WebApi.Shared.Validation;

public static class ValidationResultExtensions
{
    public static ValidationErrorResult ToApiResponse(this ValidationResult validationResult)
    {
        return new ValidationErrorResult(validationResult.Errors.Select(e => new ValidationError(e.PropertyName, e.ErrorMessage)));
    }
}