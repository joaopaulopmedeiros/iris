using StackExchange.Redis;

namespace Iris.WebApi.Shared.Infra.Redis.Extensions;

public static class IDatabaseExtensions
{
    public static async Task<SortedSetEntry[]> GetSortedSetRangeByScoreAsync(
        this IDatabase db,
        string key,
        DateOnly from,
        DateOnly to)
    {
        long start = from.ToDateTime(TimeOnly.MinValue).Ticks;
        long end = to.ToDateTime(TimeOnly.MinValue).Ticks;

        return await db.SortedSetRangeByScoreWithScoresAsync(key, start, end);
    }
}