using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;

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
            return Result<EventDescription>.Fail(errors.ToArray());
        }

        return Result<EventDescription>.Ok(new EventDescription(normalizedValue));
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
            ? EventErrors.EventDescription.TooLong
            : null;
    }

    public override string ToString() => Value;
}