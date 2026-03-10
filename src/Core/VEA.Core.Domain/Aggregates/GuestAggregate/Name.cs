using System.Text.RegularExpressions;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;

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
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result<Name>.Fail(GuestErrors.Name.Empty);
        }
        
        var normalizedValue = Normalize(value);
        
        var errors = Validate(normalizedValue);

        if (errors.Count > 0)
        {
            return Result<Name>.Fail(errors.ToArray());
        }

        return Result<Name>.Ok(new Name(normalizedValue));
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
        CheckTooShort,
        CheckTooLong,
        CheckLettersOnly
    ];
    

    private static Error? CheckTooShort(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Length < MinLength
            ? GuestErrors.Name.TooShort
            : null;
    }

    private static Error? CheckTooLong(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Length > MaxLength
            ? GuestErrors.Name.TooLong
            : null;
    }
    
    private static Error? CheckLettersOnly(string value)
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