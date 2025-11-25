namespace Iris.WebApi.Modules.Indicators.Features.Ingestion.Models;

public record struct IndicatorConfig(
    string Code,
    string BcbSeriesCode,
    string CronExpression,
    string DisplayName)
{
    public string RedisKey => $"indicator:{Code?.ToLower()}";
}

public static class IndicatorConfigs
{
    public static readonly IndicatorConfig Selic = new(
        Code: "selic",
        BcbSeriesCode: "bcdata.sgs.11",
        CronExpression: Cron.Daily(),
        DisplayName: "Taxa SELIC"
    );

    public static readonly IndicatorConfig Ipca = new(
        Code: "ipca",
        BcbSeriesCode: "bcdata.sgs.10844",
        CronExpression: Cron.Monthly(),
        DisplayName: "IPCA"
    );

    public static readonly IndicatorConfig[] All = [Selic, Ipca];

    public static IndicatorConfig? GetByCode(string code)
        => All.FirstOrDefault(c => c.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
}