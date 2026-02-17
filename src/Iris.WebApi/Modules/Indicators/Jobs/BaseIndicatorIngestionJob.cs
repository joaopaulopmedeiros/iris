using Iris.WebApi.Modules.Indicators.Mappers;
using Iris.WebApi.Modules.Indicators.Models;
using Iris.WebApi.Modules.Indicators.Repositories;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Loggers;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

using Refit;

namespace Iris.WebApi.Modules.Indicators.Jobs;

[AutomaticRetry(Attempts = 1)]
public abstract class BaseIndicatorIngestionJob<TJob>(
    ILogger<TJob> logger,
    IBCBHttpClient httpClient,
    IIndicatorTimeSeriesRepository repository) where TJob : BaseIndicatorIngestionJob<TJob>
{
    protected abstract IndicatorConfig Config { get; }
    protected readonly IBCBHttpClient _httpClient = httpClient;

    public async Task ExecuteAsync()
    {
        await repository.EnsureTimeSeriesExistsAsync(Config);

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        DateOnly from = await repository.GetNextIngestionDateAsync(Config, today);
        DateOnly to = today;

        try
        {
            IEnumerable<RawIndicator> indicators = await GetIndicatorDataAsync(new IndicatorQueryParams(from, to));

            if (indicators is null || !indicators.Any())
            {
                logger.LogNoDataFound(from, to);
                return;
            }

            await repository.AppendAsync(Config, IndicatorMapper.Map(indicators));
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            logger.LogNoDataFound(from, to);
        }
        catch (ApiException ex)
        {
            logger.LogUnhandledException((int)ex.StatusCode, ex.Content ?? string.Empty);
            throw;
        }
    }

    protected abstract Task<IEnumerable<RawIndicator>> GetIndicatorDataAsync(IndicatorQueryParams queryParams);
}