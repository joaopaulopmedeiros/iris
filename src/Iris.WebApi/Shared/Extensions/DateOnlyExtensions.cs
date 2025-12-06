namespace Iris.WebApi.Shared.Extensions;

public static class DateOnlyExtensions
{
    public static long ToUnixMilliseconds(this DateOnly date)
    {
        return new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue)).ToUnixTimeMilliseconds();
    }
}