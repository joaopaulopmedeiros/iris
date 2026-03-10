using FluentValidation;

using Iris.WebApi.Modules.Indicators.Endpoints.GetByRange;
using Iris.WebApi.Modules.Indicators.Jobs;
using Iris.WebApi.Modules.Indicators.Models;
using Iris.WebApi.Modules.Indicators.Repositories;

namespace Iris.WebApi.Modules.Indicators.IoC;

public static class DependencyInjector
{
    public static IServiceCollection AddIndicatorsModule(this IServiceCollection services)
    {
        services.AddScoped<IIndicatorTimeSeriesRepository, RedisTimeSeriesRepository>();
        services.AddScoped<IValidator<GetIndicatorsByRangeRequest>, GetIndicatorsByRangeRequestValidator>();
        services.AddScoped<SelicIngestionJob>();
        services.AddScoped<IpcaIngestionJob>();

        return services;
    }

    public static IApplicationBuilder UseIndicatorsModule(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        IRecurringJobManager jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        jobManager.AddOrUpdate<SelicIngestionJob>(
            recurringJobId: "selic-ingestion-job",
            methodCall: job => job.ExecuteAsync(),
            cronExpression: IndicatorConfigs.Selic.CronExpression,
            new RecurringJobOptions()
            {
                TimeZone = TimeZoneInfo.Local
            }
        );

        jobManager.AddOrUpdate<IpcaIngestionJob>(
            recurringJobId: "ipca-ingestion-job",
            methodCall: job => job.ExecuteAsync(),
            cronExpression: IndicatorConfigs.Ipca.CronExpression,
            new RecurringJobOptions()
            {
                TimeZone = TimeZoneInfo.Local
            }
        );

        return app;
    }

    public static WebApplication MapIndicatorsEndpoints(this WebApplication app)
    {
        app.MapGetIndicatorsByRangeEndpoint();
        return app;
    }
}