using FluentValidation;

using Iris.WebApi.Modules.Indicators.Features.Ingestion;

namespace Iris.WebApi.Modules.Indicators.Features.GetByRange;

public class GetIndicatorsByRangeRequestValidator : AbstractValidator<GetIndicatorsByRangeRequest>
{
    public GetIndicatorsByRangeRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage("Code is required.")
            .Must(IndicatorConfigs.IsValidCode)
            .WithMessage($"Invalid code. Allowed values are: {string.Join(", ", IndicatorConfigs.All.Select(c => c.Code))}");

        RuleFor(x => x.From)
            .LessThanOrEqualTo(x => x.To)
            .WithMessage("From date must be less than or equal to To date.");
    }
}
