using VEA.Core.Tools.OperationResult;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate;

public readonly record struct EventId(Guid Value)
{
    public static EventId New() => new(Guid.NewGuid());

    public static Result<EventId> From(Guid value)
        => value == Guid.Empty
            ? EventErrors.EventId.Empty
            : Result<EventId>.Ok(new EventId(value));

    public static Result<EventId> From(string value)
        => Guid.TryParse(value, out var guid)
            ? From(guid)
            : EventErrors.EventId.InvalidFormat;

    public override string ToString() => Value.ToString();
}