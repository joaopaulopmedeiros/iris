using Iris.WebApi.Shared.Infra.Http.Clients.BCB.Models;

using Microsoft.Extensions.Options;

using Refit;

namespace Iris.WebApi.Shared.Infra.Http.Clients.BCB.IoC;

public static class DependencyInjector
{
    public static IServiceCollection AddBCBHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<BCBHttpClientOptions>(configuration.GetSection("HttpClients:BCB"));

        services
            .AddRefitClient<IBCBHttpClient>()
            .ConfigureHttpClient((sp, c) =>
            {
                var options = sp.GetRequiredService<IOptions<BCBHttpClientOptions>>().Value;
                c.BaseAddress = new Uri(options.BaseUrl);
                c.Timeout = TimeSpan.FromSeconds(options.TimeoutInSeconds);
            });

        return services;
    }
}
