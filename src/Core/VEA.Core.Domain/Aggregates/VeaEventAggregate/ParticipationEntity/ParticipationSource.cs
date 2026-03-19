using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;

public sealed record ParticipationSource
{
    public ParticipationSourceValue Value { get; }

    public static ParticipationSource Public => new(ParticipationSourceValue.Public);
    public static ParticipationSource Private => new(ParticipationSourceValue.Private);

    private ParticipationSource(ParticipationSourceValue value)
    {
        Value = value;
    }

    public static Result<ParticipationSource> Create(ParticipationSourceValue value)
    {
        return Enum.IsDefined(value)
            ? new Success<ParticipationSource>(new ParticipationSource(value))
            : ParticipationErrors.ParticipationSource.Invalid;
    }

    public override string ToString() => Value.ToString();
}