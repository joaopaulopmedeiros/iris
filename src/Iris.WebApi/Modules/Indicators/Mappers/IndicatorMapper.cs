using System.Globalization;

using Iris.WebApi.Modules.Indicators.Models;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

using StackExchange.Redis;

namespace Iris.WebApi.Modules.Indicators.Mappers;

public static class IndicatorMapper
{
    public static IEnumerable<Indicator> Map(RedisResult[] results)
    {
        return results.Select(entry =>
        {
            RedisResult[] parts = (RedisResult[])entry!;

            long timestamp = (long)parts[0];

            DateOnly parsedDate = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime);

            decimal parsedValue = decimal.Parse(parts[1]!.ToString()!, NumberStyles.Float, CultureInfo.InvariantCulture);

            return new Indicator(parsedDate, parsedValue);
        });
    }

    public static Indicator Map(RawIndicator raw) => new(raw.Date, raw.Value);

    public static IEnumerable<Indicator> Map(IEnumerable<RawIndicator> raw) =>
        raw.Select(Map);
}