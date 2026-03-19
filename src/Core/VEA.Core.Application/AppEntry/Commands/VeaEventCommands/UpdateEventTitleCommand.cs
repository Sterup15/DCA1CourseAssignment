using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;

namespace VEA.Core.Application.AppEntry.Commands.VeaEventCommands;

public record UpdateEventTitleCommand
{
    public EventId EventId { get; }
    public EventTitle Title { get; }

    private UpdateEventTitleCommand(EventId eventId, EventTitle title)
    {
        EventId = eventId;
        Title = title;
    }

    public static Result<UpdateEventTitleCommand> Create(string eventId, string title)
        => ResultExtensions.CombineInto(
            EventId.From(eventId),
            EventTitle.Create(title),
            (id, t) => new UpdateEventTitleCommand(id, t));
}
