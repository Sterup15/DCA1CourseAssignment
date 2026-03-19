using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Commands.VeaEventCommands;

public record ActivateEventCommand
{
    public EventId EventId { get; }

    private ActivateEventCommand(EventId eventId)
    {
        EventId = eventId;
    }

    public static Result<ActivateEventCommand> Create(string eventId)
        => EventId.From(eventId)
            .Map(id => new ActivateEventCommand(id));
}
