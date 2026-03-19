using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.GuestAggregate;

public readonly record struct GuestId(Guid Value)
{
    public static GuestId New() => new(Guid.NewGuid());

    public static Result<GuestId> From(Guid value)
        => value == Guid.Empty
            ? GuestErrors.GuestId.Empty
            : new Success<GuestId>(new GuestId(value));

    public override string ToString() => Value.ToString();
}