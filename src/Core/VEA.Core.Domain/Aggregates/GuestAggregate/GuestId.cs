using VEA.Core.Tools.OperationResult;

namespace VEA.Core.Domain.Aggregates.GuestAggregate;

public readonly record struct GuestId(Guid Value)
{
    public static GuestId New() => new(Guid.NewGuid());

    public static Result<GuestId> From(Guid value)
        => value == Guid.Empty
            ? GuestErrors.GuestId.Empty
            : Result<GuestId>.Ok(new GuestId(value));

    public override string ToString() => Value.ToString();
}