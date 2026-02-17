using Iris.WebApi.Modules.Indicators.Models;
using Iris.WebApi.Modules.Indicators.Repositories;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

namespace Iris.WebApi.Modules.Indicators.Jobs;

public class SelicIngestionJob(
    ILogger<SelicIngestionJob> logger,
    IBCBHttpClient httpClient,
    IIndicatorTimeSeriesRepository repository) : BaseIndicatorIngestionJob<SelicIngestionJob>(logger, httpClient, repository)
{
    protected override IndicatorConfig Config => IndicatorConfigs.Selic;

    protected override async Task<IEnumerable<RawIndicator>> GetIndicatorDataAsync(IndicatorQueryParams queryParams)
    {
        return await _httpClient.GetSelicAsync(queryParams);
    }
}