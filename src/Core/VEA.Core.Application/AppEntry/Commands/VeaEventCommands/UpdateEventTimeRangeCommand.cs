using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Application.AppEntry.Commands.VeaEventCommands;

public record UpdateEventTimeRangeCommand
{
    public EventId EventId { get; }
    public EventTimeRange TimeRange { get; }

    private UpdateEventTimeRangeCommand(EventId eventId, EventTimeRange timeRange)
    {
        EventId = eventId;
        TimeRange = timeRange;
    }

    public static Result<UpdateEventTimeRangeCommand> Create(string eventId, DateTime start, DateTime end)
        => ResultExtensions.CombineInto(
            EventId.From(eventId),
            EventTimeRange.Create(start, end),
            (id, tr) => new UpdateEventTimeRangeCommand(id, tr));
}
