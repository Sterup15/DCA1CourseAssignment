using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate;

public sealed record EventVisibility
{
    public EventVisibilityValue Value { get; }

    public static EventVisibility Private => new(EventVisibilityValue.Private);
    public static EventVisibility Public => new(EventVisibilityValue.Public);

    public static EventVisibility Default => Private;

    private EventVisibility(EventVisibilityValue value)
    {
        Value = value;
    }

    public static Result<EventVisibility> Create(EventVisibilityValue value)
    {
        return Enum.IsDefined(value)
            ? new Success<EventVisibility>(new EventVisibility(value))
            : EventErrors.EventVisibility.Invalid;
    }

    public override string ToString() => Value.ToString();
}