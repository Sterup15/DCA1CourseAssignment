using System.Text.RegularExpressions;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.GuestAggregate;

public sealed record Name
{
    private const int MinLength = 2;
    private const int MaxLength = 25;

    public string Value { get; }

    private Name(string value)
    {
        Value = value;
    }

    public static Result<Name> Create(string? value)
    {
        return new Success<string>(value ?? string.Empty)
            .Ensure(v => !string.IsNullOrWhiteSpace(v), GuestErrors.Name.Empty)
            .Map(Normalize)
            .Bind(v => Validate(v) is { Count: > 0 } errors
                ? (Result<string>)new Failure<string>(errors)
                : new Success<string>(v))
            .Map(v => new Name(v));
    }

    private static string Normalize(string? value)
    {
        var trimmed = value?.Trim() ?? string.Empty;

        if (string.IsNullOrEmpty(trimmed))
        {
            return string.Empty;
        }

        return char.ToUpperInvariant(trimmed[0]) +
               trimmed[1..].ToLowerInvariant();
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
        CheckTooShort,
        CheckTooLong,
        CheckLettersOnly
    ];
    

    private static ResultError? CheckTooShort(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Length < MinLength
            ? GuestErrors.Name.TooShort
            : null;
    }

    private static ResultError? CheckTooLong(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Length > MaxLength
            ? GuestErrors.Name.TooLong
            : null;
    }
    
    private static ResultError? CheckLettersOnly(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var isValid = Regex.IsMatch(value, "^[a-zA-Z]+$");

        return !isValid
            ? GuestErrors.Name.InvalidCharacters
            : null;
    }

    public override string ToString() => Value;
}