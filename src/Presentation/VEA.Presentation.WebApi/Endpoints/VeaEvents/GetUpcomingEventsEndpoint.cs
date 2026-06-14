using Microsoft.AspNetCore.Mvc;
using VEA.Core.QueryContracts.Queries;
using VEA.Core.QueryContracts.QueryDispatching;
using VEA.Core.Tools.ObjectMapper;
using VEA.Presentation.WebApi.Endpoints.Common;

namespace VEA.Presentation.WebApi.Endpoints.VeaEvents;

public record GetUpcomingEventsRequest(int Page, int PageSize, string? Title);

public record GetUpcomingEventsResponse(IEnumerable<EventSummaryDto> Events, int TotalPages);

public record EventSummaryDto(
    string Id, string Title, string Description,
    string Start, string End,
    int AttendeeCount, int GuestCapacity, bool IsPublic);

[Route("api/events")]
public class GetUpcomingEventsEndpoint(IQueryDispatcher dispatcher, IObjectMapper mapper)
    : ApiEndpoint.WithRequest<GetUpcomingEventsRequest>.AndResponse<GetUpcomingEventsResponse>
{
    [HttpGet]
    public override async Task<ActionResult<GetUpcomingEventsResponse>> HandleAsync(
        [FromQuery] GetUpcomingEventsRequest request, CancellationToken ct = default)
    {
        var query = mapper.Map<GetUpcomingEventsRequest, UpcomingEventsPageQuery>(request);
        var answer = await dispatcher.DispatchAsync(query);
        var response = mapper.Map<UpcomingEventsPageAnswer, GetUpcomingEventsResponse>(answer);
        return Ok(response);
    }
}