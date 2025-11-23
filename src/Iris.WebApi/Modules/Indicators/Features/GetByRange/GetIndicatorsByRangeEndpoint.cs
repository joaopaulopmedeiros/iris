using System.Text.Json;

using Iris.WebApi.Modules.Indicators.Models;

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
            IDatabase db = redis.GetDatabase();

            string key = $"indicator:{request.Code}";
            long fromScore = long.Parse(request.From.ToString("yyyyMMdd"));
            long toScore = long.Parse(request.To.ToString("yyyyMMdd"));

            RedisValue[] entries = await db.SortedSetRangeByScoreAsync(key, fromScore, toScore);

            if (entries is null || entries.Length == 0) return Results.NoContent();

            JsonSerializerOptions options = new()
            {
                PropertyNameCaseInsensitive = true
            };

            IEnumerable<Indicator> indicators = entries.Select(e => JsonSerializer.Deserialize<Indicator>((string)e!, options));

            GetIndicatorsByRangeResponse response = new(
                Code: request.Code,
                Data: indicators
            );

            return Results.Ok(response);
        })
        .WithTags("Indicators");

        return app;
    }
}
