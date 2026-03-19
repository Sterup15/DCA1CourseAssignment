using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.LocationAggregate;

public readonly record struct LocationId(Guid Value)
{
    public static LocationId New() => new(Guid.NewGuid());

    public static Result<LocationId> From(Guid value)
        => value == Guid.Empty
            ? LocationErrors.LocationId.Empty
            : new Success<LocationId>(new LocationId(value));

    public override string ToString() => Value.ToString();
}