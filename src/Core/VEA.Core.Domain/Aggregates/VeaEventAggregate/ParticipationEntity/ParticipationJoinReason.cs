using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;

public sealed record ParticipationJoinReason
{
    private const int MaxLength = 100;

    public string Value { get; }

    public static ParticipationJoinReason Default => new(string.Empty);

    private ParticipationJoinReason(string value)
    {
        Value = value;
    }

    public static Result<ParticipationJoinReason> Create(string? value)
    {
        var normalizedValue = value?.Trim() ?? string.Empty;

        var errors = Validate(normalizedValue);

        if (errors.Count > 0)
        {
            return Result<ParticipationJoinReason>.Fail(errors.ToArray());
        }

        return Result<ParticipationJoinReason>.Ok(new ParticipationJoinReason(normalizedValue));
    }

    private static IReadOnlyList<Error> Validate(string value)
    {
        var errors = new List<Error>();

        foreach (var rule in Rules)
        {
            var error = rule(value);

            if (error is not null)
            {
                errors.Add(error);
            }
        }

        return errors;
    }

    private delegate Error? ValidationRule(string value);

    private static readonly IReadOnlyList<ValidationRule> Rules =
    [
        CheckTooLong
    ];

    private static Error? CheckTooLong(string value)
    {
        return value.Length > MaxLength
            ? ParticipationErrors.ParticipationJoinReason.TooLong
            : null;
    }

    public override string ToString() => Value;
}