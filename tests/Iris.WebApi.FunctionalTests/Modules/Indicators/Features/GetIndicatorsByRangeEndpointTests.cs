using System.Net;
using System.Net.Http.Json;

using Iris.WebApi.FunctionalTests.Modules.Indicators.Seeders;
using Iris.WebApi.Modules.Indicators.Endpoints.GetByRange;

using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

using StackExchange.Redis;

namespace Iris.WebApi.FunctionalTests.Modules.Indicators.Features;

public sealed class GetIndicatorsByRangeEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>, IAsyncDisposable
{
    private const string TestKey = "indicator:selic";

    [Fact]
    public async Task GetIndicatorsByRange_ShouldReturnOk_WhenDataExists()
    {
        await IndicatorSeeder.SeedTestDataAsync(factory.Services, TestKey);

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

    [Fact]
    public async Task GetIndicatorsByRange_ShouldReturnBadRequest_WhenInvalidCodeIsPassed()
    {
        DateOnly from = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        DateOnly to = DateOnly.FromDateTime(DateTime.Now);

        string url = $"/indicators?code=invalid_code&from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";
        HttpClient httpClient = factory.CreateClient();

        HttpResponseMessage response = await httpClient.GetAsync(url, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    public async ValueTask DisposeAsync()
    {
        using var scope = factory.Services.CreateScope();
        var redis = scope.ServiceProvider.GetRequiredService<IConnectionMultiplexer>();
        var db = redis.GetDatabase();
        await db.KeyDeleteAsync(TestKey);
    }
}