using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Commands.VeaEventCommands;

public record MakeEventPublicCommand
{
    public EventId EventId { get; }

    private MakeEventPublicCommand(EventId eventId)
    {
        EventId = eventId;
    }

    public static Result<MakeEventPublicCommand> Create(string eventId)
        => EventId.From(eventId)
            .Map(id => new MakeEventPublicCommand(id));
}
