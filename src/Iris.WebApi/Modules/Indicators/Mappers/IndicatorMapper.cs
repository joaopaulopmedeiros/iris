using System.Globalization;

using Iris.WebApi.Modules.Indicators.Models;

using StackExchange.Redis;

namespace Iris.WebApi.Modules.Indicators.Mappers;

public static class IndicatorMapper
{
    public static IEnumerable<Indicator> Map(RedisResult[] results)
    {
        return results.Select(entry =>
        {
            var parts = (RedisResult[])entry!;
            var timestamp = (long)parts[0];
            var parsedDate = DateOnly.FromDateTime(DateTimeOffset.FromUnixTimeMilliseconds(timestamp).DateTime);
            var parsedValue = decimal.Parse(parts[1]!.ToString()!, NumberStyles.Float, CultureInfo.InvariantCulture);

            return new Indicator(parsedDate, parsedValue);
        });
    }
}