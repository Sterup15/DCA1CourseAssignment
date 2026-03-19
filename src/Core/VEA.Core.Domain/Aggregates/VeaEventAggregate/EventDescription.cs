using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate;

public sealed record EventDescription
{
    private const int MaxLength = 250;

    public string Value { get; }
    
    // Only for internal hardcoded defaults, otherwise use the Create method to include validation, as seen in EventTitle
    public static EventDescription Default => new(string.Empty);

    private EventDescription(string value)
    {
        Value = value;
    }

    public static Result<EventDescription> Create(string? value)
    {
        var normalizedValue = value?.Trim() ?? string.Empty;
        var errors = Validate(normalizedValue);

        if (errors.Count > 0)
        {
            return new Failure<EventDescription>(errors);
        }

        return new Success<EventDescription>(new EventDescription(normalizedValue));
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
            ? EventErrors.EventDescription.TooLong
            : null;
    }

    public override string ToString() => Value;
}