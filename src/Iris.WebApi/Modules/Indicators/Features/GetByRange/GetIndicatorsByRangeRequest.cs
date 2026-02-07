namespace Iris.WebApi.Modules.Indicators.Features.GetByRange;

public readonly record struct GetIndicatorsByRangeRequest(
    string Code,
    DateOnly From,
    DateOnly To
);