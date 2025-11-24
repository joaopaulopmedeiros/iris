using System.Globalization;

using Iris.WebApi.Shared.Infra.Http.Clients.BCB;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Loggers;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

using Refit;

using StackExchange.Redis;

namespace Iris.WebApi.Modules.Indicators.Features.Ingestion;

[AutomaticRetry(Attempts = 1)]
public class SelicIngestionJob(
    ILogger<SelicIngestionJob> logger,
    IBCBHttpClient httpClient,
    IConnectionMultiplexer redis)
{
    private const string Key = "indicator:selic";

    public async Task ExecuteAsync()
    {
        logger.LogInformation("Executing selic ingestion job...");

        IDatabase db = redis.GetDatabase();

        if (await db.KeyExistsAsync(Key) == false)
        {
            await db.ExecuteAsync("TS.CREATE", Key, "RETENTION", 0, "LABELS", "code", "selic");
        }

        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        DateOnly from = await GetStartDateAsync(db, today);
        DateOnly to = today;

        try
        {
            IEnumerable<Indicator> indicators = await httpClient.GetSelicAsync(new IndicatorQueryParams(from, to));

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

    private static async Task<DateOnly> GetStartDateAsync(IDatabase db, DateOnly today)
    {
        RedisResult result = await db.ExecuteAsync("TS.GET", Key);

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
    private static async Task AddIndicatorsAsync(IDatabase db, IEnumerable<Indicator> indicators)
    {
        List<object> args = [];

        foreach (var i in indicators)
        {
            long timestamp = new DateTimeOffset(i.Date.ToDateTime(TimeOnly.MinValue))
                .ToUnixTimeMilliseconds();

            args.Add(Key);
            args.Add(timestamp);
            args.Add(i.Value.ToString(CultureInfo.InvariantCulture));
        }

        await db.ExecuteAsync("TS.MADD", [.. args]);
    }
}