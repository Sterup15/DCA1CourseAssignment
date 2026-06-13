using Microsoft.EntityFrameworkCore;
using VEA.Core.QueryContracts.Contract;
using VEA.Core.QueryContracts.Queries;

namespace VEA.Infrastructure.EfcQueries.Queries;

public class ViewSingleEventQueryHandler(VeadatabaseProductionContext ctx)
    : IQueryHandler<ViewSingleEventQuery, ViewSingleEventAnswer>
{
    public async Task<ViewSingleEventAnswer> HandleAsync(ViewSingleEventQuery query)
    {
        var eventInfo = await ctx.Events
            .Where(e => e.Id == query.EventId)
            .Select(e => new
            {
                e.Id, e.Title, e.Description, e.Location,
                e.TimeRangeStart, e.TimeRangeEnd, e.Visibility, e.GuestCapacity,
                AttendeeCount = e.Participations.Count(p => p.Status == "Attending")
            })
            .SingleAsync();

        var attendees = await ctx.Participations
            .Where(p => p.EventId == query.EventId && p.Status == "Attending")
            .OrderBy(p => p.Guest.LastName)
            .Skip(query.GuestOffset)
            .Take(query.GuestPageSize)
            .Select(p => new AttendeeItem(
                p.GuestId,
                p.Guest.FirstName + " " + p.Guest.LastName,
                p.Guest.ProfilePictureUrl))
            .ToListAsync();

        return new ViewSingleEventAnswer(
            eventInfo.Id, eventInfo.Title, eventInfo.Description,
            eventInfo.Location, eventInfo.TimeRangeStart!, eventInfo.TimeRangeEnd!,
            eventInfo.Visibility == 1, eventInfo.AttendeeCount, eventInfo.GuestCapacity,
            attendees);
    }
}