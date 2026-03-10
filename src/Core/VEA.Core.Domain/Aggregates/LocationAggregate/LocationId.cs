using VEA.Core.Tools.OperationResult;

namespace VEA.Core.Domain.Aggregates.LocationAggregate;

public readonly record struct LocationId(Guid Value)
{
    public static LocationId New() => new(Guid.NewGuid());

    public static Result<LocationId> From(Guid value)
        => value == Guid.Empty
            ? LocationErrors.LocationId.Empty
            : Result<LocationId>.Ok(new LocationId(value));

    public override string ToString() => Value.ToString();
}