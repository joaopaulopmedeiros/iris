using System.Text.Json;

using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Converters;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

using Microsoft.Extensions.Options;

using Refit;

namespace Iris.WebApi.Shared.Infra.Http.Clients.BCB.IoC;

public static class DependencyInjector
{
    public static IServiceCollection AddBCBHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BCBHttpClientOptions>(configuration.GetSection("HttpClients:BCB"));

        JsonSerializerOptions options = new();
        options.Converters.Add(new DateOnlyJsonConverter());
        options.Converters.Add(new DecimalStringJsonConverter());

        services
            .AddRefitClient<IBCBHttpClient>()
            .ConfigureHttpClient((sp, c) =>
            {
                var options = sp.GetRequiredService<IOptions<BCBHttpClientOptions>>().Value;
                c.BaseAddress = new Uri(options.BaseUrl);
                c.Timeout = TimeSpan.FromSeconds(options.TimeoutInSeconds);
            })
            .AddTypedClient(c => RestService.For<IBCBHttpClient>(c, new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(options)
            }));

        return services;
    }
}
