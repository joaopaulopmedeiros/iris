using System.Globalization;

using Microsoft.Extensions.DependencyInjection;

using StackExchange.Redis;

namespace Iris.WebApi.FunctionalTests.Modules.Indicators.Seeders;

public static class IndicatorSeeder
{
    public static async Task SeedTestDataAsync(IServiceProvider serviceProvider, string code)
    {
        string testKey = $"indicator:{code}";
        using var scope = serviceProvider.CreateScope();
        IConnectionMultiplexer redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
        IDatabase db = redis.GetDatabase();

        if (await db.KeyExistsAsync(testKey) == false)
        {
            await db.ExecuteAsync("TS.CREATE", testKey, "RETENTION", 0, "LABELS", "code", code);
        }

        List<(long timestamp, decimal value)> testData = GenerateTestData(15);

        List<object> args = [];

        foreach (var (timestamp, value) in testData)
        {
            args.Add(testKey);
            args.Add(timestamp);
            args.Add(value.ToString(CultureInfo.InvariantCulture));
        }

        if (args.Count > 0)
        {
            await db.ExecuteAsync("TS.MADD", [.. args]);
        }
    }

    private static List<(long timestamp, decimal value)> GenerateTestData(int days)
    {
        List<(long, decimal)> data = [];
        Random random = new(42);

        for (int i = days; i >= 0; i--)
        {
            DateTime date = DateTime.Now.AddDays(-i).Date;
            long timestamp = new DateTimeOffset(date).ToUnixTimeMilliseconds();
            decimal value = 10.0m + (decimal)(random.NextDouble() * 3.0);

            data.Add((timestamp, Math.Round(value, 2)));
        }

        return data;
    }
}