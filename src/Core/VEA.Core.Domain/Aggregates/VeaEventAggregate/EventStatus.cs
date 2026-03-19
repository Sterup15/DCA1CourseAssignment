using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate;

public sealed record EventStatus
{
    public EventStatusValue Value { get; }

    public static EventStatus Draft => new(EventStatusValue.Draft);
    public static EventStatus Ready => new(EventStatusValue.Ready);
    public static EventStatus Active => new(EventStatusValue.Active);
    public static EventStatus Cancelled => new(EventStatusValue.Cancelled);

    public static EventStatus Default => Draft;

    private EventStatus(EventStatusValue value)
    {
        Value = value;
    }

    public static Result<EventStatus> Create(EventStatusValue value)
    {
        return Enum.IsDefined(value)
            ? new Success<EventStatus>(new EventStatus(value))
            : new Failure<EventStatus>([EventErrors.EventStatus.Invalid]);
    }

    public Result<EventStatus> ChangeTo(EventStatus nextStatus)
    {
        var error = GetTransitionError(Value, nextStatus.Value);

        if (error is not null)
        {
            return new Failure<EventStatus>([error]);
        }

        return new Success<EventStatus>(nextStatus);
    }

    private static ResultError? GetTransitionError(EventStatusValue current, EventStatusValue next)
    {
        if (!Enum.IsDefined(current) || !Enum.IsDefined(next))
        {
            return EventErrors.EventStatus.Invalid;
        }

        var isAllowed = (current, next) switch
        {
            (EventStatusValue.Draft, EventStatusValue.Ready) => true,
            (EventStatusValue.Draft, EventStatusValue.Cancelled) => true,

            (EventStatusValue.Ready, EventStatusValue.Active) => true,
            (EventStatusValue.Ready, EventStatusValue.Cancelled) => true,

            (EventStatusValue.Active, EventStatusValue.Active) => true,
            (EventStatusValue.Active, EventStatusValue.Cancelled) => true,

            _ => false
        };

        return isAllowed
            ? null
            : EventErrors.EventStatus.InvalidTransition(current, next);
    }

    public override string ToString() => Value.ToString();
}