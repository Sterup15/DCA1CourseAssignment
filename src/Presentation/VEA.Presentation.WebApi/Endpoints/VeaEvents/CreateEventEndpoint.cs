using Microsoft.AspNetCore.Mvc;
using VEA.Core.Application.AppEntry;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using DomainEventId = VEA.Core.Domain.Aggregates.VeaEventAggregate.EventId;
using VEA.Core.Tools.OperationResult.Result;
using VEA.Presentation.WebApi.Endpoints.Common;

namespace VEA.Presentation.WebApi.Endpoints.VeaEvents;

public record CreateEventResponse(string EventId);

[Route("api/events")]
public class CreateEventEndpoint(ICommandHandler<CreateEventCommand, DomainEventId> handler)
    : ApiEndpoint.WithResponse<CreateEventResponse>
{
    [HttpPost]
    public override async Task<ActionResult<CreateEventResponse>> HandleAsync(CancellationToken ct = default)
    {
        var result = await handler.HandleAsync(new CreateEventCommand());
        return result switch
        {
            Success<DomainEventId> s => Created($"api/events/{s.Value}", new CreateEventResponse(s.Value.ToString())),
            Failure<DomainEventId> f => UnprocessableEntity(f.Errors.Select(e => e.Message)),
            _ => StatusCode(500)
        };
    }
}