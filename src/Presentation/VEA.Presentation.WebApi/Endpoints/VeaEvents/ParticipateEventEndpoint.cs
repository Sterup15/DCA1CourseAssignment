using Microsoft.AspNetCore.Mvc;
using VEA.Core.Application.AppEntry;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Tools.OperationResult.Result;
using VEA.Presentation.WebApi.Endpoints.Common;

namespace VEA.Presentation.WebApi.Endpoints.VeaEvents;

public record ParticipateEventRequest(string GuestId, string? JoinReason);

[Route("api/events/{eventId}/participations")]
public class ParticipateEventEndpoint(ICommandHandler<ParticipateEventAsGuestCommand, None> handler)
    : ApiEndpoint.WithRequest<ParticipateEventRequest>
{
    [HttpPost]
    public override async Task<IActionResult> HandleAsync(
        [FromBody] ParticipateEventRequest request, CancellationToken ct = default)
    {
        var eventId = RouteData.Values["eventId"]?.ToString() ?? "";

        var cmdResult = ParticipateEventAsGuestCommand.Create(eventId, request.GuestId, request.JoinReason);
        if (cmdResult is Failure<ParticipateEventAsGuestCommand> cmdFailure)
            return BadRequest(cmdFailure.Errors.Select(e => e.Message));

        var cmd = ((Success<ParticipateEventAsGuestCommand>)cmdResult).Value;
        var result = await handler.HandleAsync(cmd);

        return result switch
        {
            Success<None> => Ok(),
            Failure<None> f => UnprocessableEntity(f.Errors.Select(e => e.Message)),
            _ => StatusCode(500)
        };
    }
}