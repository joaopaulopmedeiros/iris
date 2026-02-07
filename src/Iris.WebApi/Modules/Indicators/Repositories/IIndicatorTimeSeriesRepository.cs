using Iris.WebApi.Modules.Indicators.Features.Ingestion;
using Iris.WebApi.Modules.Indicators.Models;

namespace Iris.WebApi.Modules.Indicators.Repositories;

public interface IIndicatorTimeSeriesRepository
{
    Task EnsureTimeSeriesExistsAsync(IndicatorConfig config);

    Task<DateOnly> GetNextIngestionDateAsync(IndicatorConfig config, DateOnly today);

    Task AppendAsync(IndicatorConfig config, IEnumerable<Indicator> indicators);

    Task<IEnumerable<Indicator>> GetIndicatorsAsync(string code, DateOnly from, DateOnly to);
}