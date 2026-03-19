using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Core.Tools.OperationResult.Result;

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
            return new Failure<ParticipationJoinReason>(errors);
        }

        return new Success<ParticipationJoinReason>(new ParticipationJoinReason(normalizedValue));
    }

    private static IReadOnlyList<ResultError> Validate(string value)
    {
        var errors = new List<ResultError>();

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

    private delegate ResultError? ValidationRule(string value);

    private static readonly IReadOnlyList<ValidationRule> Rules =
    [
        CheckTooLong
    ];

    private static ResultError? CheckTooLong(string value)
    {
        return value.Length > MaxLength
            ? ParticipationErrors.ParticipationJoinReason.TooLong
            : null;
    }

    public override string ToString() => Value;
}