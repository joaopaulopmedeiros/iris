namespace Iris.WebApi.Shared.Infra.Http.Clients.BCB.Loggers;

public static partial class LoggerExtensions
{
    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Nenhum dado encontrado entre {From:dd/MM/yyyy} e {To:dd/MM/yyyy}.")]
    public static partial void LogNoDataFound(this ILogger logger, DateOnly from, DateOnly to);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Erro inesperado ao consultar BCB. Status: {StatusCode}, Detalhe: {Detail}")]
    public static partial void LogUnhandledException(this ILogger logger, int statusCode, string detail);
}