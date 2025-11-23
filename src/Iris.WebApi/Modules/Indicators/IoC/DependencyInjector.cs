using Iris.WebApi.Modules.Indicators.Features.GetByRange;

namespace Iris.WebApi.Modules.Indicators.IoC;

public static class DependencyInjector
{
    public static WebApplication MapIndicatorsEndpoints(this WebApplication app)
    {
        app.MapGetIndicatorsByRangeEndpoint();
        return app;
    }
}