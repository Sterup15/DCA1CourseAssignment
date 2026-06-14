using Microsoft.AspNetCore.Mvc;
using VEA.Core.Application.AppEntry;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Tools.OperationResult.Result;
using VEA.Presentation.WebApi.Endpoints.Common;

namespace VEA.Presentation.WebApi.Endpoints.VeaEvents;

public record UpdateEventTitleRequest(string Title);

[Route("api/events/{eventId}/title")]
public class UpdateEventTitleEndpoint(ICommandHandler<UpdateEventTitleCommand, None> handler)
    : ApiEndpoint<UpdateEventTitleRequest>
{
    [HttpPut]
    public override async Task<IActionResult> HandleAsync(
        [FromBody] UpdateEventTitleRequest request, CancellationToken ct = default)
    {
        var eventId = RouteData.Values["eventId"]?.ToString() ?? "";

        var cmdResult = UpdateEventTitleCommand.Create(eventId, request.Title);
        if (cmdResult is Failure<UpdateEventTitleCommand> cmdFailure)
            return BadRequest(cmdFailure.Errors.Select(e => e.Message));

        var cmd = ((Success<UpdateEventTitleCommand>)cmdResult).Value;
        var result = await handler.HandleAsync(cmd);

        return result switch
        {
            Success<None> => Ok(),
            Failure<None> f => UnprocessableEntity(f.Errors.Select(e => e.Message)),
            _ => StatusCode(500)
        };
    }
}