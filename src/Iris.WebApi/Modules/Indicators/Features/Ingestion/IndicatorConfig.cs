namespace Iris.WebApi.Modules.Indicators.Features.Ingestion;

public record struct IndicatorConfig(
    string Code,
    string BcbSeriesCode,
    string CronExpression,
    string DisplayName)
{
    public readonly string RedisKey => $"indicator:{Code?.ToLower()}";
}

public static class IndicatorConfigs
{
    public static readonly IndicatorConfig Selic = new(
        Code: "selic",
        BcbSeriesCode: "bcdata.sgs.11",
        CronExpression: Cron.Minutely(),
        DisplayName: "Taxa SELIC"
    );

    public static readonly IndicatorConfig Ipca = new(
        Code: "ipca",
        BcbSeriesCode: "bcdata.sgs.10844",
        CronExpression: Cron.Minutely(),
        DisplayName: "IPCA"
    );

    public static readonly IndicatorConfig[] All = [Selic, Ipca];

    public static IndicatorConfig GetByCode(string code)
        => All.First(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));

    public static bool IsValidCode(string code)
        => All.Any(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
}