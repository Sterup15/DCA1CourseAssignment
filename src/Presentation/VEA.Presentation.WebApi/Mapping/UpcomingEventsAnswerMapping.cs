using VEA.Core.QueryContracts.Queries;
using VEA.Core.Tools.ObjectMapper;
using VEA.Presentation.WebApi.Endpoints.VeaEvents;

namespace VEA.Presentation.WebApi.Mapping;

public class UpcomingEventsAnswerMapping : IMapping<UpcomingEventsPageAnswer, GetUpcomingEventsResponse>
{
    public GetUpcomingEventsResponse Map(UpcomingEventsPageAnswer input)
        => new(
            input.Events.Select(e => new EventSummaryDto(
                e.Id, e.Title, e.Description,
                e.Start, e.End,
                e.AttendeeCount, e.GuestCapacity, e.IsPublic)),
            input.TotalPages);
}