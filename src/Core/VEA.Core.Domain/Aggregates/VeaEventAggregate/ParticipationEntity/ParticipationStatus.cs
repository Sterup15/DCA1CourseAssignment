using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;

public sealed record ParticipationStatus
{
    public ParticipationStatusValue Value { get; }

    public static ParticipationStatus Invited => new(ParticipationStatusValue.Invited);
    public static ParticipationStatus Rejected => new(ParticipationStatusValue.Rejected);
    public static ParticipationStatus Attending => new(ParticipationStatusValue.Attending);

    private ParticipationStatus(ParticipationStatusValue value)
    {
        Value = value;
    }

    public static Result<ParticipationStatus> Create(ParticipationStatusValue value)
    {
        return Enum.IsDefined(value)
            ? new Success<ParticipationStatus>(new ParticipationStatus(value))
            : ParticipationErrors.ParticipationStatus.Invalid;
    }

    public override string ToString() => Value.ToString();
}