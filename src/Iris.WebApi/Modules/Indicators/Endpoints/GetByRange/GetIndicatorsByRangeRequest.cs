namespace Iris.WebApi.Modules.Indicators.Endpoints.GetByRange;

public readonly record struct GetIndicatorsByRangeRequest(
    string Code,
    DateOnly From,
    DateOnly To
);