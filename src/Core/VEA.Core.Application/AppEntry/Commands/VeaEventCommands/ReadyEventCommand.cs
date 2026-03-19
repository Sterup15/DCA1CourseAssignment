using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Commands.VeaEventCommands;

public record ReadyEventCommand
{
    public EventId EventId { get; }

    private ReadyEventCommand(EventId eventId)
    {
        EventId = eventId;
    }

    public static Result<ReadyEventCommand> Create(string eventId)
        => EventId.From(eventId)
            .Map(id => new ReadyEventCommand(id));
}
