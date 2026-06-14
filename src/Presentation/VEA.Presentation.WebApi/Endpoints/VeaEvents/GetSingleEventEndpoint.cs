using Microsoft.AspNetCore.Mvc;
using VEA.Core.QueryContracts.Queries;
using VEA.Core.QueryContracts.QueryDispatching;
using VEA.Core.Tools.ObjectMapper;
using VEA.Presentation.WebApi.Endpoints.Common;

namespace VEA.Presentation.WebApi.Endpoints.VeaEvents;

public record GetSingleEventRequest(int GuestOffset = 0, int GuestPageSize = 10);

public record GetSingleEventResponse(
    string Id, string Title, string Description, string? Location,
    string Start, string End, bool IsPublic,
    int AttendeeCount, int GuestCapacity,
    IEnumerable<AttendeeSummaryDto> Attendees);

public record AttendeeSummaryDto(string Id, string FullName, string ProfilePictureUrl);

[Route("api/events/{eventId}")]
public class GetSingleEventEndpoint(IQueryDispatcher dispatcher, IObjectMapper mapper)
    : ApiEndpoint<GetSingleEventRequest, GetSingleEventResponse>
{
    [HttpGet]
    public override async Task<ActionResult<GetSingleEventResponse>> HandleAsync(
        [FromQuery] GetSingleEventRequest request, CancellationToken ct = default)
    {
        var eventId = RouteData.Values["eventId"]?.ToString() ?? "";
        var query = new ViewSingleEventQuery(eventId, request.GuestOffset, request.GuestPageSize);
        var answer = await dispatcher.DispatchAsync(query);
        var response = mapper.Map<ViewSingleEventAnswer, GetSingleEventResponse>(answer);
        return Ok(response);
    }
}