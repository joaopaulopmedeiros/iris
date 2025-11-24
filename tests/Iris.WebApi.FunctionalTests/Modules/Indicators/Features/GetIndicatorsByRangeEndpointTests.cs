using Iris.WebApi.Modules.Indicators.Features.GetByRange;

using Microsoft.AspNetCore.Mvc.Testing;

using System.Net;
using System.Net.Http.Json;

namespace Iris.WebApi.FunctionalTests.Modues.Indicators.Features;

public class GetIndicatorsByRangeEndpointTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetIndicatorsByRange_ShouldReturnOk_WhenDataExists()
    {
        DateOnly from = DateOnly.FromDateTime(DateTime.Now.AddDays(-10));
        DateOnly to = DateOnly.FromDateTime(DateTime.Now);

        string url = $"/indicators?code=selic&from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}";

        HttpClient httpClient = factory.CreateClient();

        HttpResponseMessage response = await httpClient.GetAsync(url, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        GetIndicatorsByRangeResponse content = await response.Content.ReadFromJsonAsync<GetIndicatorsByRangeResponse>(TestContext.Current.CancellationToken);

        Assert.NotEmpty(content.Data);
        Assert.Equal("selic", content.Code);
    }
}