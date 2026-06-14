using Microsoft.AspNetCore.Mvc;
using VEA.Core.Application.AppEntry;
using VEA.Core.Application.AppEntry.Commands.GuestCommands;
using VEA.Core.Tools.OperationResult.Result;
using VEA.Presentation.WebApi.Endpoints.Common;

namespace VEA.Presentation.WebApi.Endpoints.Guests;

public record RegisterGuestRequest(string FirstName, string LastName, string Email);

[Route("api/guests")]
public class RegisterGuestEndpoint(ICommandHandler<RegisterGuestCommand, None> handler)
    : ApiEndpoint.WithRequest<RegisterGuestRequest>
{
    [HttpPost]
    public override async Task<IActionResult> HandleAsync(
        [FromBody] RegisterGuestRequest request, CancellationToken ct = default)
    {
        var cmdResult = RegisterGuestCommand.Create(request.FirstName, request.LastName, request.Email);
        if (cmdResult is Failure<RegisterGuestCommand> cmdFailure)
            return BadRequest(cmdFailure.Errors.Select(e => e.Message));

        var cmd = ((Success<RegisterGuestCommand>)cmdResult).Value;
        var result = await handler.HandleAsync(cmd);

        return result switch
        {
            Success<None> => Created(),
            Failure<None> f => UnprocessableEntity(f.Errors.Select(e => e.Message)),
            _ => StatusCode(500)
        };
    }
}