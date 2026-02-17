using Iris.WebApi.Modules.Indicators.Models;

namespace Iris.WebApi.Modules.Indicators.Endpoints.GetByRange;

public readonly record struct GetIndicatorsByRangeResponse(
    string Code,
    IEnumerable<Indicator> Data
);