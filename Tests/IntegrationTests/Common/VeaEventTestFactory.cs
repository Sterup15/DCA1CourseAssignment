using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult.Result;

namespace IntegrationTests.Common;

public static class VeaEventTestFactory
{
    public static VeaEvent CreateEvent()
    {
        var result = VeaEvent.Create();
        return Assert.IsType<Success<VeaEvent>>(result).Value;
    }

    public static EventTitle CreateTitle(string value)
    {
        var result = EventTitle.Create(value);
        return Assert.IsType<Success<EventTitle>>(result).Value;
    }

    public static EventDescription CreateDescription(string value)
    {
        var result = EventDescription.Create(value);
        return Assert.IsType<Success<EventDescription>>(result).Value;
    }

    public static EventTimeRange CreateTimeRange(DateTime start, DateTime end)
    {
        var result = EventTimeRange.Create(start, end);
        return Assert.IsType<Success<EventTimeRange>>(result).Value;
    }

    public static EventGuestCapacity CreateGuestCapacity(int value)
    {
        var result = EventGuestCapacity.Create(value);
        return Assert.IsType<Success<EventGuestCapacity>>(result).Value;
    }

    public static VeaEvent CreateReadyEvent()
    {
        var veaEvent = CreateEvent();
        var now = DefaultNow;

        veaEvent.UpdateTitle(CreateTitle("Valid Title"));
        veaEvent.UpdateDescription(CreateDescription("Valid description"));
        veaEvent.UpdateTimeRange(
            CreateTimeRange(
                new DateTime(2030, 08, 25, 13, 00, 00, DateTimeKind.Utc),
                new DateTime(2030, 08, 25, 15, 00, 00, DateTimeKind.Utc)),
            now);

        var readyResult = veaEvent.MakeReady(now);
        Assert.IsType<Success<VeaEvent>>(readyResult);

        return veaEvent;
    }

    public static VeaEvent CreateActiveEvent()
    {
        var veaEvent = CreateReadyEvent();
        var now = DefaultNow;

        var activeResult = veaEvent.MakeActive(now);
        Assert.IsType<Success<VeaEvent>>(activeResult);

        return veaEvent;
    }

    public static DateTime DefaultNow =>
        new(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);
}
