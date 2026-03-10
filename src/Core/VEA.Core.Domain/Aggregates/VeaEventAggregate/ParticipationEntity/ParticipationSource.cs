using VEA.Core.Tools.OperationResult;

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
            ? Result<ParticipationSource>.Ok(new ParticipationSource(value))
            : ParticipationErrors.ParticipationSource.Invalid;
    }

    public override string ToString() => Value.ToString();
}