using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate;

public sealed record EventTitle
{
    private const int MinLength = 3;
    private const int MaxLength = 75;

    public string Value { get; }
    
    // Example for if the default comes from external input, then validation is needed, otherwise it makes more noise than value.
    public static Result<EventTitle> Default => Create("Working Title");

    private EventTitle(string value)
    {
        Value = value;
    }

    public static Result<EventTitle> Create(string? value)
    {
        var normalizedValue = value?.Trim();

        if (string.IsNullOrWhiteSpace(normalizedValue))
        {
            return Result<EventTitle>.Fail(EventErrors.EventTitle.Empty);
        }
        
        var errors = Validate(normalizedValue);

        if (errors.Count > 0)
        {
            return Result<EventTitle>.Fail(errors.ToArray());
        }

        return Result<EventTitle>.Ok(new EventTitle(normalizedValue!));
    }

    private static IReadOnlyList<Error> Validate(string? value)
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

    private delegate Error? ValidationRule(string? value);

    private static readonly IReadOnlyList<ValidationRule> Rules =
    [
        CheckTooShort,
        CheckTooLong
    ];

    private static Error? CheckTooShort(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Length < MinLength
            ? EventErrors.EventTitle.TooShort
            : null;
    }

    private static Error? CheckTooLong(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Length > MaxLength
            ? EventErrors.EventTitle.TooLong
            : null;
    }

    public override string ToString() => Value;
}