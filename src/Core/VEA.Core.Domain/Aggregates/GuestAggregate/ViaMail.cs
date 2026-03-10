using System.Text.RegularExpressions;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;

namespace VEA.Core.Domain.Aggregates.GuestAggregate;

public sealed record ViaMail
{
    private const string Domain = "@via.dk";

    public string Value { get; }

    private ViaMail(string value)
    {
        Value = value;
    }

    public static Result<ViaMail> Create(string? value)
    {
        var normalizedValue = value?.Trim().ToLowerInvariant() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(normalizedValue))
        {
            return Result<ViaMail>.Fail(GuestErrors.ViaMail.Empty);
        }

        var errors = Validate(normalizedValue);

        if (errors.Count > 0)
        {
            return Result<ViaMail>.Fail(errors.ToArray());
        }

        return Result<ViaMail>.Ok(new ViaMail(normalizedValue));
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
        CheckDomain,
        CheckLocalPartFormat
    ];

    private static Error? CheckDomain(string value)
    {
        return value.EndsWith(Domain)
            ? null
            : GuestErrors.ViaMail.InvalidDomain;
    }

    private static Error? CheckLocalPartFormat(string value)
    {
        if (!value.EndsWith(Domain))
        {
            return null;
        }

        var localPart = value[..^Domain.Length];

        var lettersPattern = @"^[a-z]{3,4}$";
        var digitsPattern = @"^\d{6}$";

        if (Regex.IsMatch(localPart, lettersPattern) ||
            Regex.IsMatch(localPart, digitsPattern))
        {
            return null;
        }

        return GuestErrors.ViaMail.InvalidFormat;
    }

    public override string ToString() => Value;
}