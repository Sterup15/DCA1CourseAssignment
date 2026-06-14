using VEA.Core.QueryContracts.Queries;
using VEA.Core.Tools.ObjectMapper;
using VEA.Presentation.WebApi.Endpoints.VeaEvents;

namespace VEA.Presentation.WebApi.Mapping;

public class SingleEventAnswerMapping : IMapping<ViewSingleEventAnswer, GetSingleEventResponse>
{
    public GetSingleEventResponse Map(ViewSingleEventAnswer input)
        => new(
            input.Id, input.Title, input.Description, input.Location,
            input.Start, input.End, input.IsPublic,
            input.AttendeeCount, input.GuestCapacity,
            input.Attendees.Select(a => new AttendeeSummaryDto(a.Id, a.FullName, a.ProfilePictureUrl)));
}