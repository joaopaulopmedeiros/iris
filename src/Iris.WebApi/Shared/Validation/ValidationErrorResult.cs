namespace Iris.WebApi.Shared.Validation;

public record ValidationErrorResult(IEnumerable<ValidationError> Errors);

public record ValidationError(string Field, string Message);