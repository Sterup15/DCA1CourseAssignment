using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Commands.VeaEventCommands;

public record MakeEventPrivateCommand
{
    public EventId EventId { get; }

    private MakeEventPrivateCommand(EventId eventId)
    {
        EventId = eventId;
    }

    public static Result<MakeEventPrivateCommand> Create(string eventId)
        => EventId.From(eventId)
            .Map(id => new MakeEventPrivateCommand(id));
}
