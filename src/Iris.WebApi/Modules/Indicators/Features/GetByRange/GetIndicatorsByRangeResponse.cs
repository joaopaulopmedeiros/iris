using Iris.WebApi.Modules.Indicators.Models;

namespace Iris.WebApi.Modules.Indicators.Features.GetByRange;

public record struct GetIndicatorsByRangeResponse(string Code, IEnumerable<Indicator> Data);