using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;

public readonly record struct ParticipationId(Guid Value)
{
    public static ParticipationId New() => new(Guid.NewGuid());

    public static Result<ParticipationId> From(Guid value)
        => value == Guid.Empty
            ? ParticipationErrors.ParticipationId.Empty
            : new Success<ParticipationId>(new ParticipationId(value));

    public override string ToString() => Value.ToString();
}