using FluentValidation;

using Iris.WebApi.Modules.Indicators.Models;

namespace Iris.WebApi.Modules.Indicators.Endpoints.GetByRange;

public class GetIndicatorsByRangeRequestValidator : AbstractValidator<GetIndicatorsByRangeRequest>
{
    private static readonly string AllowedCodes = string.Join(", ", IndicatorConfigs.All.Select(c => c.Code));

    public GetIndicatorsByRangeRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.")
            .Must(IndicatorConfigs.IsValidCode)
            .WithMessage($"Invalid code. Allowed values are: {AllowedCodes}");

        RuleFor(x => x.From)
            .NotEmpty()
            .WithMessage("From date is required.")
            .LessThanOrEqualTo(x => x.To)
            .WithMessage("From date must be less than or equal to To date.");

        RuleFor(x => x.To)
            .NotEmpty()
            .WithMessage("To date is required.");
    }
}