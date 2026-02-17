using System.Net;
using System.Net.Http.Json;

using Iris.WebApi.FunctionalTests.Fixtures;
using Iris.WebApi.FunctionalTests.Modules.Indicators.Seeders;
using Iris.WebApi.Modules.Indicators.Endpoints.GetByRange;

namespace Iris.WebApi.FunctionalTests.Modules.Indicators.Endpoints;

[Collection(WebApiCollectionFixture.Name)]
public sealed class GetIndicatorsByRangeEndpointTests(
    WebApiTestFixture<Program> fixture)
{
    [Theory]
    [InlineData("selic")]
    [InlineData("ipca")]
    public async Task Endpoint_ShouldReturnOk_WhenDataExists(string code)
    {
        await IndicatorSeeder.SeedTestDataAsync(fixture.Factory.Services, code);

        DateOnly from = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        DateOnly to = DateOnly.FromDateTime(DateTime.Now);

        string url = $"/indicators?code={code}&from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";

        HttpClient httpClient = fixture.HttpClient;

        HttpResponseMessage response = await httpClient.GetAsync(url, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GetIndicatorsByRangeResponse content =
            await response.Content.ReadFromJsonAsync<GetIndicatorsByRangeResponse>(TestContext.Current.CancellationToken);

        Assert.NotEmpty(content!.Data);
        Assert.Equal(code, content.Code);
    }

    [Fact]
    public async Task Endpoint_ShouldReturnBadRequest_WhenInvalidCodeIsPassed()
    {
        DateOnly from = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        DateOnly to = DateOnly.FromDateTime(DateTime.Now);

        string url = $"/indicators?code=invalid_code&from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";

        HttpClient httpClient = fixture.HttpClient;

        HttpResponseMessage response = await httpClient.GetAsync(url, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}