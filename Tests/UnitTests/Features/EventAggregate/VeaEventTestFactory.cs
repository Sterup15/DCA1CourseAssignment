using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;

namespace UnitTests.Features.EventAggregate;

public static class VeaEventTestFactory
{
    public static VeaEvent CreateEvent()
    {
        var result = VeaEvent.Create();
        return Assert.IsType<Result<VeaEvent>.Success>(result).Value;
    }

    public static EventTitle CreateTitle(string value)
    {
        var result = EventTitle.Create(value);
        return Assert.IsType<Result<EventTitle>.Success>(result).Value;
    }

    public static EventDescription CreateDescription(string value)
    {
        var result = EventDescription.Create(value);
        return Assert.IsType<Result<EventDescription>.Success>(result).Value;
    }

    public static EventTimeRange CreateTimeRange(DateTime start, DateTime end)
    {
        var result = EventTimeRange.Create(start, end);
        return Assert.IsType<Result<EventTimeRange>.Success>(result).Value;
    }

    public static EventGuestCapacity CreateGuestCapacity(int value)
    {
        var result = EventGuestCapacity.Create(value);
        return Assert.IsType<Result<EventGuestCapacity>.Success>(result).Value;
    }

    public static VeaEvent CreateReadyEvent()
    {
        var veaEvent = CreateEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        veaEvent.UpdateTitle(CreateTitle("Valid Title"));
        veaEvent.UpdateDescription(CreateDescription("Valid description"));
        veaEvent.UpdateTimeRange(
            CreateTimeRange(
                new DateTime(2030, 08, 25, 13, 00, 00, DateTimeKind.Utc),
                new DateTime(2030, 08, 25, 15, 00, 00, DateTimeKind.Utc)),
            now);

        var readyResult = veaEvent.MakeReady(now);
        Assert.IsType<Result<VeaEvent>.Success>(readyResult);

        return veaEvent;
    }

    public static VeaEvent CreateActiveEvent()
    {
        var veaEvent = CreateReadyEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        var activeResult = veaEvent.MakeActive(now);
        Assert.IsType<Result<VeaEvent>.Success>(activeResult);

        return veaEvent;
    }
    
    public static VeaEvent CreateEventWithStatus(EventStatusValue status) =>
        status switch
        {
            EventStatusValue.Draft => CreateEvent(),
            EventStatusValue.Ready => CreateReadyEvent(),
            EventStatusValue.Active => CreateActiveEvent(),
            _ => throw new ArgumentOutOfRangeException(nameof(status), status, null)
        };
    
    public static DateTime Parse(string value) =>
        DateTime.SpecifyKind(DateTime.Parse(value), DateTimeKind.Utc);
    
    public static Result<EventGuestCapacity> CreateGuestCapacityOrFailure(int value) =>
        EventGuestCapacity.Create(value);
    
    public static DateTime DefaultNow =>
        new(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);
    
    public static VeaEvent CreateCancelledEvent()
    {
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var cancelResult = veaEvent.Cancel();
        Assert.IsType<Result<VeaEvent>.Success>(cancelResult);
        return veaEvent;
    }
    
}