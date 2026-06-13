using Microsoft.EntityFrameworkCore;
using VEA.Core.QueryContracts.Contract;
using VEA.Core.QueryContracts.Queries;

namespace VEA.Infrastructure.EfcQueries.Queries;

public class GuestProfilePageQueryHandler(VeadatabaseProductionContext ctx, ISystemTime time)
    : IQueryHandler<GuestProfilePageQuery, GuestProfilePageAnswer>
{
    public async Task<GuestProfilePageAnswer> HandleAsync(GuestProfilePageQuery query)
    {
        string now = time.CurrentTime().ToString("yyyy-MM-ddTHH:mm:ss");

        var guest = await ctx.Guests
            .Where(g => g.Id == query.GuestId)
            .Select(g => new { g.Id, g.FirstName, g.LastName, g.ViaMail, g.ProfilePictureUrl })
            .SingleAsync();

        int pendingCount = await ctx.Participations
            .Where(p => p.GuestId == query.GuestId && p.Status == "Invited")
            .CountAsync();

        var upcoming = await ctx.Participations
            .Where(p => p.GuestId == query.GuestId
                     && p.Status == "Attending"
                     && p.Event.TimeRangeStart != null
                     && p.Event.TimeRangeStart.CompareTo(now) > 0)
            .OrderBy(p => p.Event.TimeRangeStart)
            .Select(p => new UpcomingEventItem(
                p.EventId,
                p.Event.Title,
                p.Event.Participations.Count(pp => pp.Status == "Attending"),
                p.Event.TimeRangeStart!))
            .ToListAsync();

        var past = await ctx.Participations
            .Where(p => p.GuestId == query.GuestId
                     && p.Status == "Attending"
                     && p.Event.TimeRangeStart != null
                     && p.Event.TimeRangeStart.CompareTo(now) <= 0)
            .OrderByDescending(p => p.Event.TimeRangeStart)
            .Take(5)
            .Select(p => new PastEventItem(p.EventId, p.Event.Title))
            .ToListAsync();

        return new GuestProfilePageAnswer(
            guest.Id,
            guest.FirstName + " " + guest.LastName,
            guest.ViaMail,
            guest.ProfilePictureUrl,
            pendingCount,
            upcoming,
            past);
    }
}