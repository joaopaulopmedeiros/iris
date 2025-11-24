using Hangfire;

using Iris.WebApi.Modules.Indicators.Features.GetByRange;
using Iris.WebApi.Modules.Indicators.Features.Ingestion;

namespace Iris.WebApi.Modules.Indicators.IoC;

public static class DependencyInjector
{
    public static IApplicationBuilder UseIndicatorsBackgroundJobs(this IApplicationBuilder app)
    {
        using IServiceScope scope = app.ApplicationServices.CreateScope();

        IRecurringJobManager jobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();

        jobManager.AddOrUpdate<SelicIngestionJob>(
            recurringJobId: "selic-ingestion-job",
            methodCall: job => job.ExecuteAsync(),
            cronExpression: Cron.Minutely(),
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