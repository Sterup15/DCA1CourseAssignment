using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate;

public sealed record EventGuestCapacity
{
    private const int MinCapacity = 5;
    private const int MaxCapacity = 50;

    public int Value { get; }

    public static EventGuestCapacity Default => new(MinCapacity);
    private EventGuestCapacity(int value)
    {
        Value = value;
    }

    public static Result<EventGuestCapacity> Create(int value)
    {
        var errors = Validate(value);

        if (errors.Count > 0)
        {
            return new Failure<EventGuestCapacity>(errors);
        }

        return new Success<EventGuestCapacity>(new EventGuestCapacity(value));
    }

    private static IReadOnlyList<ResultError> Validate(int value)
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

    private delegate ResultError? ValidationRule(int value);

    private static readonly IReadOnlyList<ValidationRule> Rules =
    [
        CheckTooSmall,
        CheckTooLarge
    ];

    private static ResultError? CheckTooSmall(int value)
    {
        return value < MinCapacity
            ? EventErrors.EventGuestCapacity.TooSmall
            : null;
    }

    private static ResultError? CheckTooLarge(int value)
    {
        return value > MaxCapacity
            ? EventErrors.EventGuestCapacity.TooLarge
            : null;
    }

    public override string ToString() => Value.ToString();
}