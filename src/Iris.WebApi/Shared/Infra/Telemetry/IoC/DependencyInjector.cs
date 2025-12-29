using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace Iris.WebApi.Shared.Infra.Telemetry.IoC;

public static class DependencyInjector
{
    public static IServiceCollection AddTelemetry(this IServiceCollection services, IConfiguration configuration)
    {
        string serviceName = "iris-api";

        string otlpEndpoint = configuration.GetValue<string>("Otlp:Endpoint")!;

        services.AddMetrics();

        OpenTelemetryBuilder otel = services.AddOpenTelemetry();

        otel.ConfigureResource(resource => resource
            .AddService(serviceName: serviceName)
            .AddTelemetrySdk());

        otel.WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation()
                .AddOtlpExporter(opt => opt.Endpoint = new Uri(otlpEndpoint)));

        otel.WithTracing(tracing => tracing
                .AddRedisInstrumentation()
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(opt => opt.Endpoint = new Uri(otlpEndpoint)));

        otel.WithLogging(logging => logging
                .AddOtlpExporter(opt => opt.Endpoint = new Uri(otlpEndpoint)));

        return services;
    }
}