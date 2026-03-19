using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Core.Tools.OperationResult.Result;

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
        return new Success<string>(normalizedValue ?? string.Empty)
            .Ensure(v => !string.IsNullOrWhiteSpace(v), EventErrors.EventTitle.Empty)
            .Bind(v => Validate(v) is { Count: > 0 } errors
                ? (Result<string>)new Failure<string>(errors)
                : new Success<string>(v))
            .Map(v => new EventTitle(v));
    }

    private static IReadOnlyList<ResultError> Validate(string? value)
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

    private delegate ResultError? ValidationRule(string? value);

    private static readonly IReadOnlyList<ValidationRule> Rules =
    [
        CheckTooShort,
        CheckTooLong
    ];

    private static ResultError? CheckTooShort(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Length < MinLength
            ? EventErrors.EventTitle.TooShort
            : null;
    }

    private static ResultError? CheckTooLong(string? value)
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