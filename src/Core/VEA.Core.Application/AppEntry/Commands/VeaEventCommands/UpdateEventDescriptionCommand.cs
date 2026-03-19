using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Commands.VeaEventCommands;

public record UpdateEventDescriptionCommand
{
    public EventId EventId { get; }
    public EventDescription Description { get; }

    private UpdateEventDescriptionCommand(EventId eventId, EventDescription description)
    {
        EventId = eventId;
        Description = description;
    }

    public static Result<UpdateEventDescriptionCommand> Create(string eventId, string description)
        => ResultExtensions.CombineInto(
            EventId.From(eventId),
            EventDescription.Create(description),
            (id, d) => new UpdateEventDescriptionCommand(id, d));
}
