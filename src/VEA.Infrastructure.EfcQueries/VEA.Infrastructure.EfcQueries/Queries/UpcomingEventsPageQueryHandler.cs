using Microsoft.EntityFrameworkCore;
using VEA.Core.QueryContracts.Contract;
using VEA.Core.QueryContracts.Queries;

namespace VEA.Infrastructure.EfcQueries.Queries;

public class UpcomingEventsPageQueryHandler(VeadatabaseProductionContext ctx, ISystemTime time)
    : IQueryHandler<UpcomingEventsPageQuery, UpcomingEventsPageAnswer>
{
    public async Task<UpcomingEventsPageAnswer> HandleAsync(UpcomingEventsPageQuery query)
    {
        string now = time.CurrentTime().ToString("yyyy-MM-ddTHH:mm:ss");

        var baseQuery = ctx.Events
            .Where(e => e.Status == "Active"
                     && e.TimeRangeStart != null
                     && e.TimeRangeStart.CompareTo(now) > 0);

        if (!string.IsNullOrEmpty(query.TitleFilter))
            baseQuery = baseQuery.Where(e => e.Title.Contains(query.TitleFilter));

        int totalCount = await baseQuery.CountAsync();
        int totalPages = (int)Math.Ceiling(totalCount / (double)query.PageSize);

        var events = await baseQuery
            .OrderBy(e => e.TimeRangeStart)
            .Skip((query.PageNumber - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(e => new UpcomingEventSummary(
                e.Id, e.Title, e.Description,
                e.TimeRangeStart!, e.TimeRangeEnd!,
                e.Participations.Count(p => p.Status == "Attending"),
                e.GuestCapacity,
                e.Visibility == 1))
            .ToListAsync();

        return new UpcomingEventsPageAnswer(events, totalPages);
    }
}