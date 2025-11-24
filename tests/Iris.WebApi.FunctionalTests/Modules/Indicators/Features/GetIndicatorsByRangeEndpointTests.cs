using System.Globalization;
using System.Net;
using System.Net.Http.Json;

using Iris.WebApi.Modules.Indicators.Features.GetByRange;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using StackExchange.Redis;

namespace Iris.WebApi.FunctionalTests.Modues.Indicators.Features;

public class GetIndicatorsByRangeEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>, IAsyncDisposable
{
    private const string TestKey = "indicator:selic";
    private readonly List<long> _insertedTimestamps = [];

    [Fact]
    public async Task GetIndicatorsByRange_ShouldReturnOk_WhenDataExists()
    {
        await SeedTestDataAsync();

        DateOnly from = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        DateOnly to = DateOnly.FromDateTime(DateTime.Now);

        string url = $"/indicators?code=selic&from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";

        HttpClient httpClient = factory.CreateClient();

        HttpResponseMessage response = await httpClient.GetAsync(url, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GetIndicatorsByRangeResponse content = await response.Content.ReadFromJsonAsync<GetIndicatorsByRangeResponse>(TestContext.Current.CancellationToken);

        Assert.NotEmpty(content.Data);
        Assert.Equal("selic", content.Code);

        Assert.All(content.Data, indicator =>
        {
            Assert.True(indicator.Date >= from);
            Assert.True(indicator.Date <= to);
        });
    }

    private async Task SeedTestDataAsync()
    {
        using var scope = factory.Services.CreateScope();
        IConnectionMultiplexer redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
        IDatabase db = redis.GetDatabase();

        if (await db.KeyExistsAsync(TestKey) == false)
        {
            await db.ExecuteAsync("TS.CREATE", TestKey, "RETENTION", 0, "LABELS", "code", "selic");
        }

        List<(long timestamp, decimal value)> testData = GenerateTestData(15);

        List<object> args = [];

        foreach (var (timestamp, value) in testData)
        {
            args.Add(TestKey);
            args.Add(timestamp);
            args.Add(value.ToString(CultureInfo.InvariantCulture));
            _insertedTimestamps.Add(timestamp);
        }

        if (args.Count > 0)
        {
            await db.ExecuteAsync("TS.MADD", args.ToArray());
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

    public async ValueTask DisposeAsync()
    {
        if (_insertedTimestamps.Count > 0)
        {
            using var scope = factory.Services.CreateScope();
            var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
            var db = redis.GetDatabase();
            await db.KeyDeleteAsync(TestKey);
        }
    }
}