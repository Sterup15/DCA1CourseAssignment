using System.Text.RegularExpressions;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.GuestAggregate;

public sealed record ViaMail
{
    private const string Domain = "@via.dk";

    public string Value { get; }

    private ViaMail(string value)
    {
        Value = value;
    }

    public static Result<ViaMail> Create(string? value, bool isEmailInUse = false)
    {
        var normalizedValue = value?.Trim().ToLowerInvariant() ?? string.Empty;
        return new Success<string>(normalizedValue)
            .Ensure(v => !string.IsNullOrWhiteSpace(v), GuestErrors.ViaMail.Empty)
            .Bind(v => Validate(v) is { Count: > 0 } errors
                ? (Result<string>)new Failure<string>(errors)
                : new Success<string>(v))
            .Ensure(_ => !isEmailInUse, GuestErrors.ViaMail.EmailInUse)
            .Map(v => new ViaMail(v));
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
        CheckDomain,
        CheckLocalPartFormat
    ];

    private static ResultError? CheckDomain(string value)
    {
        return value.EndsWith(Domain)
            ? null
            : GuestErrors.ViaMail.InvalidDomain;
    }

    private static ResultError? CheckLocalPartFormat(string value)
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