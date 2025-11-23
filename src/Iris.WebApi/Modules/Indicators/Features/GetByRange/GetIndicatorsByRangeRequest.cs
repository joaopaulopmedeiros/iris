namespace Iris.WebApi.Modules.Indicators.Features.GetByRange;

public record struct GetIndicatorsByRangeRequest(string Code, DateOnly From, DateOnly To);
