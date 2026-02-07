using FluentValidation;
using FluentValidation.Results;

using Iris.WebApi.Modules.Indicators.Features.Ingestion.Models;
using Iris.WebApi.Modules.Indicators.Mappers;
using Iris.WebApi.Modules.Indicators.Models;
using Iris.WebApi.Shared.Validation;

using StackExchange.Redis;

namespace Iris.WebApi.Modules.Indicators.Features.GetByRange;

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
        IDatabase redis)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return Results.BadRequest(validationResult.ToApiResponse());
        }

        RedisResult timeSeries = await redis.ExecuteAsync(
            "TS.RANGE",
            IndicatorConfigs.GetByCode(request.Code).RedisKey,
            request.From.ToUnixMilliseconds(),
            request.To.ToUnixMilliseconds());

        if (timeSeries.IsNull || timeSeries.Length == 0)
        {
            return Results.NoContent();
        }

        IEnumerable<Indicator> data = IndicatorMapper.Map((RedisResult[])timeSeries!);

        return Results.Ok(new GetIndicatorsByRangeResponse(request.Code, data));
    }
}