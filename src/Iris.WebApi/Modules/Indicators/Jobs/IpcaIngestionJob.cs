using Iris.WebApi.Modules.Indicators.Models;
using Iris.WebApi.Modules.Indicators.Repositories;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

namespace Iris.WebApi.Modules.Indicators.Jobs;

public class IpcaIngestionJob(
    ILogger<IpcaIngestionJob> logger,
    IBCBHttpClient httpClient,
    IIndicatorTimeSeriesRepository repository) : BaseIndicatorIngestionJob<IpcaIngestionJob>(logger, httpClient, repository)
{
    protected override IndicatorConfig Config => IndicatorConfigs.Ipca;

    protected override async Task<IEnumerable<RawIndicator>> GetIndicatorDataAsync(IndicatorQueryParams queryParams)
    {
        return await _httpClient.GetIpcaAsync(queryParams);
    }
}