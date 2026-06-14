using Microsoft.AspNetCore.Mvc;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.AppEntry.Dispatcher;
using VEA.Core.Tools.OperationResult.Result;
using VEA.Presentation.WebApi.Endpoints.Common;
using VeaEventId = VEA.Core.Domain.Aggregates.VeaEventAggregate.EventId;

namespace VEA.Presentation.WebApi.Endpoints.VeaEvents;

public record CreateEventResponse(string EventId);

[Route("api/events")]
public class CreateEventEndpoint(ICommandDispatcher dispatcher)
    : ApiEndpoint.WithResponse<CreateEventResponse>
{
    [HttpPost]
    public override async Task<ActionResult<CreateEventResponse>> HandleAsync(CancellationToken ct = default)
    {
        var result = await dispatcher.DispatchAsync<CreateEventCommand, VeaEventId>(new CreateEventCommand());
        return result switch
        {
            Success<VeaEventId> s => Created($"api/events/{s.Value}", new CreateEventResponse(s.Value.ToString())),
            Failure<VeaEventId> f => UnprocessableEntity(f.Errors.Select(e => e.Message)),
            _ => StatusCode(500)
        };
    }
}
