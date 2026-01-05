using System.Diagnostics;

using Iris.WebApi.Modules.Indicators.Features.Ingestion.Models;
using Iris.WebApi.Modules.Indicators.Mappers;
using Iris.WebApi.Modules.Indicators.Models;

using StackExchange.Redis;

namespace Iris.WebApi.Modules.Indicators.Features.GetByRange;

public static class GetIndicatorsByRangeEndpoint
{
    private static readonly ActivitySource ActivitySource = new("Iris.Indicators");
    public static WebApplication MapGetIndicatorsByRangeEndpoint(this WebApplication app)
    {
        app.MapGet("/indicators", async (
            [AsParameters] GetIndicatorsByRangeRequest request,
            IDatabase redis) =>
        {
            using var endpointActivity = ActivitySource.StartActivity("GetIndicatorsByRange.Handler");

            using var configActivity = ActivitySource.StartActivity("GetConfig");
            IndicatorConfig? config = IndicatorConfigs.GetByCode(request.Code);
            configActivity?.Dispose();

            if (config?.Code is null)
            {
                string validCodes = string.Join(", ", IndicatorConfigs.All.Select(c => c.Code));
                return Results.BadRequest(new { Message = $"Invalid indicator code. Valid codes: {validCodes}" });
            }

            using var redisActivity = ActivitySource.StartActivity("Redis.TS.RANGE");
            redisActivity?.SetTag("redis.key", config.Value.RedisKey);
            redisActivity?.SetTag("redis.from", request.From.ToString());
            redisActivity?.SetTag("redis.to", request.To.ToString());

            RedisResult timeSeries = await redis.ExecuteAsync(
                "TS.RANGE",
                config.Value.RedisKey,
                request.From.ToUnixMilliseconds(),
                request.To.ToUnixMilliseconds(),
                "AGGREGATION", "last", 86400000); // 1 dia em ms
            redisActivity?.Dispose();

            if (timeSeries.IsNull || timeSeries.Length == 0) return Results.NoContent();

            using var mapperActivity = ActivitySource.StartActivity("Mapper.MapIndicators");
            mapperActivity?.SetTag("indicator.code", request.Code);
            IEnumerable<Indicator> data = IndicatorMapper.Map((RedisResult[])timeSeries!);
            mapperActivity?.SetTag("indicator.count", data.Count());
            mapperActivity?.Dispose();

            using var responseActivity = ActivitySource.StartActivity("CreateResponse");
            GetIndicatorsByRangeResponse response = new(request.Code, data);
            var result = Results.Ok(response);
            responseActivity?.Dispose();

            return result;
        })
        .WithTags("Indicators");

        return app;
    }
}