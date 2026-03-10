using VEA.Core.Tools.OperationResult;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;

public readonly record struct ParticipationId(Guid Value)
{
    public static ParticipationId New() => new(Guid.NewGuid());

    public static Result<ParticipationId> From(Guid value)
        => value == Guid.Empty
            ? ParticipationErrors.ParticipationId.Empty
            : Result<ParticipationId>.Ok(new ParticipationId(value));

    public override string ToString() => Value.ToString();
}