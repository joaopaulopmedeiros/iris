using System.Globalization;

using Iris.WebApi.Modules.Indicators.Features.Ingestion.Models;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Loggers;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

using Refit;

using StackExchange.Redis;

namespace Iris.WebApi.Modules.Indicators.Features.Ingestion;

[AutomaticRetry(Attempts = 1)]
public abstract class BaseIndicatorIngestionJob<TJob>(
    ILogger<TJob> logger,
    IBCBHttpClient httpClient,
    IConnectionMultiplexer redis) where TJob : BaseIndicatorIngestionJob<TJob>
{
    protected abstract IndicatorConfig Config { get; }
    protected readonly IBCBHttpClient _httpClient = httpClient;

    public async Task ExecuteAsync()
    {
        IDatabase db = redis.GetDatabase();

        if (!await db.KeyExistsAsync(Config.RedisKey))
        {
            await db.ExecuteAsync("TS.CREATE", Config.RedisKey, "RETENTION", 0, "LABELS", "code", Config.Code.ToLower());
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        DateOnly from = await GetStartDateAsync(db, today);
        DateOnly to = today;

        try
        {
            IEnumerable<Indicator> indicators = await GetIndicatorDataAsync(new IndicatorQueryParams(from, to));

            if (indicators is null || !indicators.Any())
            {
                logger.LogNoDataFound(from, to);
                return;
            }

            await AddIndicatorsAsync(db, indicators);
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

    protected abstract Task<IEnumerable<Indicator>> GetIndicatorDataAsync(IndicatorQueryParams queryParams);

    private async Task<DateOnly> GetStartDateAsync(IDatabase db, DateOnly today)
    {
        try
        {
            RedisResult result = await db.ExecuteAsync("TS.GET", Config.RedisKey);

            if (result.IsNull)
            {
                return today.AddYears(-10);
            }

            RedisResult[] parts = (RedisResult[])result!;

            if (parts == null || parts.Length == 0)
            {
                return today.AddYears(-10);
            }

            long lastTimestamp = (long)parts[0];

            DateOnly lastDate = DateOnly.FromDateTime(
                DateTimeOffset.FromUnixTimeMilliseconds(lastTimestamp).DateTime
            );

            return lastDate.AddDays(1);
        }
        catch (Exception)
        {
            return today.AddYears(-10);
        }
    }

    private async Task AddIndicatorsAsync(IDatabase db, IEnumerable<Indicator> indicators)
    {
        var indicatorsArray = indicators.ToArray();
        if (indicatorsArray.Length == 0) return;

        var args = new object[indicatorsArray.Length * 3];
        int index = 0;

        foreach (var indicator in indicatorsArray)
        {
            long timestamp = new DateTimeOffset(indicator.Date.ToDateTime(TimeOnly.MinValue))
                .ToUnixTimeMilliseconds();

            args[index++] = Config.RedisKey;
            args[index++] = timestamp;
            args[index++] = indicator.Value.ToString(CultureInfo.InvariantCulture);
        }

        await db.ExecuteAsync("TS.MADD", args);
    }
}