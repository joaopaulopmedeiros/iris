using Iris.WebApi.Shared.Infra.Hangfire.IoC;
using Iris.WebApi.Shared.Infra.Http.Clients.BCB.IoC;
using Iris.WebApi.Shared.Infra.Redis.IoC;
using Iris.WebApi.Shared.Infra.Telemetry.IoC;

namespace Iris.WebApi.Shared.Infra.IoC;

public static class DependencyInjector
{
    public static IServiceCollection AddSharedInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration,
        bool includeTelemetry = true)
    {
        if (includeTelemetry)
        {
            services.AddSharedTelemetry(configuration);
        }

        services.AddRedis(configuration);
        services.AddHangfire(configuration);
        services.AddBCBHttpClient(configuration);

        return services;
    }
}