using FluentValidation;
using FluentValidation.Results;

using Iris.WebApi.Modules.Indicators.Models;
using Iris.WebApi.Modules.Indicators.Repositories;
using Iris.WebApi.Shared.Validation;

namespace Iris.WebApi.Modules.Indicators.Endpoints.GetByRange;

public static class GetIndicatorsByRangeEndpoint
{
    public static WebApplication MapGetIndicatorsByRangeEndpoint(this WebApplication app)
    {
        app.MapGet("/indicators", HandleAsync)
            .WithTags("Indicators")
            .CacheOutput(policy => policy.SetVaryByQuery("code", "from", "to"));

        return app;
    }

    private static async Task<IResult> HandleAsync(
        HttpContext context,
        [AsParameters] GetIndicatorsByRangeRequest request,
        IValidator<GetIndicatorsByRangeRequest> validator,
        IIndicatorTimeSeriesRepository repository)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.ToApiResponse());
        }

        IEnumerable<Indicator> indicators = await repository
            .GetIndicatorsAsync(request.Code, request.From, request.To);

        return indicators is null || !indicators.Any()
            ? Results.NoContent()
            : Results.Ok(new GetIndicatorsByRangeResponse(request.Code, indicators));
    }
}