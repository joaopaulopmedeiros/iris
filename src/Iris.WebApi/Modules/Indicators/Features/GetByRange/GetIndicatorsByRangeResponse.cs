using Iris.WebApi.Modules.Indicators.Models;

namespace Iris.WebApi.Modules.Indicators.Features.GetByRange;

public readonly record struct GetIndicatorsByRangeResponse(
    string Code,
    IEnumerable<Indicator> Data
);