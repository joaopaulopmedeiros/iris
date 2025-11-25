using Iris.WebApi.Modules.Indicators.Features.Ingestion.Models;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

using StackExchange.Redis;

namespace Iris.WebApi.Modules.Indicators.Features.Ingestion;

public class IpcaIngestionJob(
    ILogger<IpcaIngestionJob> logger,
    IBCBHttpClient httpClient,
    IConnectionMultiplexer redis) : BaseIndicatorIngestionJob<IpcaIngestionJob>(logger, httpClient, redis)
{
    protected override IndicatorConfig Config => IndicatorConfigs.Ipca;

    protected override async Task<IEnumerable<Indicator>> GetIndicatorDataAsync(IndicatorQueryParams queryParams)
    {
        return await _httpClient.GetIpcaAsync(queryParams);
    }
}