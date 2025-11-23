using Iris.WebApi.Modules.Indicators.GetByRange;
using Iris.WebApi.Modules.Indicators.GetStats;

namespace Iris.WebApi.Modules.Indicators.IoC;

public static class DependencyInjector
{
    public static WebApplication MapIndicatorsEndpoints(this WebApplication app)
    {
        app.MapGetIndicatorsStatsEndpoint();
        app.MapGetIndicatorsByRangeEndpoint();
        return app;
    }
}