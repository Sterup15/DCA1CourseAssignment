using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Errors;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Core.Domain.Aggregates.VeaEventAggregate;

public sealed record EventTimeRange
{
    private static readonly TimeOnly EarliestStartTime = new(8, 0);
    private static readonly TimeOnly LatestNextDayEndTime = new(1, 0);
    private static readonly TimeSpan MaxDuration = TimeSpan.FromHours(10);
    private static readonly TimeSpan MinDuration = TimeSpan.FromHours(1);

    public DateTime Start { get; }
    public DateTime End { get; }

    private EventTimeRange(DateTime start, DateTime end)
    {
        Start = start;
        End = end;
    }

    public static Result<EventTimeRange> Create(DateTime start, DateTime end)
    {
        return new Success<(DateTime, DateTime)>((start, end))
            .Ensure(t => t.Item1 != default, EventErrors.EventTimeRange.StartEmpty)
            .Ensure(t => t.Item2 != default, EventErrors.EventTimeRange.EndEmpty)
            .Bind(t => Validate(t.Item1, t.Item2) is { Count: > 0 } errors
                ? (Result<(DateTime, DateTime)>)new Failure<(DateTime, DateTime)>(errors)
                : new Success<(DateTime, DateTime)>(t))
            .Map(t => new EventTimeRange(t.Item1, t.Item2));
    }

    private static IReadOnlyList<ResultError> Validate(DateTime start, DateTime end)
    {
        var errors = new List<ResultError>();

        foreach (var rule in Rules)
        {
            var error = rule(start, end);

            if (error is not null)
            {
                errors.Add(error);
            }
        }

        return errors;
    }

    private delegate ResultError? ValidationRule(DateTime start, DateTime end);

    private static readonly IReadOnlyList<ValidationRule> Rules =
    [
        CheckDurationIsAtLeast1Hour,
        CheckEndIsAfterStart,
        CheckStartIsNotBefore08,
        CheckEndIsWithinAllowedWindow,
        CheckDurationIsAtMost10Hours
    ];
    
    private static ResultError? CheckEndIsAfterStart(DateTime start, DateTime end)
    {
        if (start == default || end == default)
        {
            return null;
        }
        
        return end <= start
            ? EventErrors.EventTimeRange.EndMustBeAfterStart
            : null;
    }

    private static ResultError? CheckStartIsNotBefore08(DateTime start, DateTime end)
    {
        if (start == default || end == default)
        {
            return null;
        }
        
        var startTime = TimeOnly.FromDateTime(start);

        return startTime < EarliestStartTime
            ? EventErrors.EventTimeRange.StartTooEarly
            : null;
    }

    private static ResultError? CheckEndIsWithinAllowedWindow(DateTime start, DateTime end)
    {
        if (start == default || end == default)
        {
            return null;
        }
        
        if (end <= start)
        {
            return null;
        }

        var startDate = DateOnly.FromDateTime(start);
        var endDate = DateOnly.FromDateTime(end);

        if (endDate == startDate)
        {
            return null;
        }

        if (endDate == startDate.AddDays(1))
        {
            var endTime = TimeOnly.FromDateTime(end);

            return endTime > LatestNextDayEndTime
                ? EventErrors.EventTimeRange.EndOutsideAllowedHours
                : null;
        }

        return EventErrors.EventTimeRange.EndOutsideAllowedHours;
    }

    private static ResultError? CheckDurationIsAtMost10Hours(DateTime start, DateTime end)
    {
        if (start == default || end == default)
        {
            return null;
        }
        
        if (end <= start)
        {
            return null;
        }

        return end - start > MaxDuration
            ? EventErrors.EventTimeRange.TooLong
            : null;
    }
    
    private static ResultError? CheckDurationIsAtLeast1Hour(DateTime start, DateTime end)
    {
        if (end < start)
        {
            return null;
        }

        return end - start < MinDuration
            ? EventErrors.EventTimeRange.TooShort
            : null;
    }

    public override string ToString() => $"{Start:yyyy-MM-dd HH:mm} - {End:yyyy-MM-dd HH:mm}";
}