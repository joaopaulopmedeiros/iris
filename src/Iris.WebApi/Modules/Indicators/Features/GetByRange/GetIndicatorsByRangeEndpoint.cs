using Iris.WebApi.Modules.Indicators.Models;
using Iris.WebApi.Shared.Infra.Redis.Extensions;

using StackExchange.Redis;

namespace Iris.WebApi.Modules.Indicators.Features.GetByRange;

public static class GetIndicatorsByRangeEndpoint
{
    public static WebApplication MapGetIndicatorsByRangeEndpoint(this WebApplication app)
    {
        app.MapGet("/indicators", async (
            [AsParameters] GetIndicatorsByRangeRequest request,
            IConnectionMultiplexer redis) =>
        {
            string key = $"indicator:{request.Code.ToLower()}";

            RedisValue[] entries = await redis.GetDatabase()
                .GetSortedSetRangeByScoreAsync(key, request.From, request.To);

            if (entries is null || entries.Length == 0) return Results.NoContent();

            IEnumerable<Indicator> data = entries.ToList<Indicator>();

            GetIndicatorsByRangeResponse response = new(
                Code: request.Code,
                Data: data
            );

            return Results.Ok(response);
        })
        .WithTags("Indicators");

        return app;
    }
}
