using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.UpdateEventTimeRange;

public class UpdateEventTimeRangeAggregateTests
{
    [Theory]
    [InlineData("2030-08-25 19:00", "2030-08-25 23:59")]
    [InlineData("2030-08-25 12:00", "2030-08-25 16:30")]
    [InlineData("2030-08-25 08:00", "2030-08-25 12:15")]
    [InlineData("2030-08-25 10:00", "2030-08-25 20:00")]
    [InlineData("2030-08-25 13:00", "2030-08-25 23:00")]
    public void UpdateTimeRange_WhenEventIsDraftAndSameDayRangeIsValid_UpdatesTimeRange(
        string startRaw,
        string endRaw)
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);
        var timeRange = VeaEventTestFactory.CreateTimeRange(VeaEventTestFactory.Parse(startRaw), VeaEventTestFactory.Parse(endRaw));

        // Act
        var result = veaEvent.UpdateTimeRange(timeRange, now);

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        Assert.Equal(timeRange, success.Value.TimeRange);
        Assert.Equal(EventStatus.Draft, success.Value.Status);
    }

    [Theory]
    [InlineData("2030-08-25 19:00", "2030-08-26 01:00")]
    [InlineData("2030-08-25 12:00", "2030-08-25 16:30")]
    [InlineData("2030-08-25 08:00", "2030-08-25 12:15")]
    public void UpdateTimeRange_WhenEventIsDraftAndCrossDayRangeIsValid_UpdatesTimeRange(
        string startRaw,
        string endRaw)
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);
        var timeRange = VeaEventTestFactory.CreateTimeRange(VeaEventTestFactory.Parse(startRaw), VeaEventTestFactory.Parse(endRaw));

        // Act
        var result = veaEvent.UpdateTimeRange(timeRange, now);

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        Assert.Equal(timeRange, success.Value.TimeRange);
        Assert.Equal(EventStatus.Draft, success.Value.Status);
    }

    [Theory]
    [InlineData("2030-08-25 19:00", "2030-08-25 23:59")]
    [InlineData("2030-08-25 19:00", "2030-08-26 01:00")]
    public void UpdateTimeRange_WhenEventIsReady_UpdatesTimeRangeAndResetsStatusToDraft(
        string startRaw,
        string endRaw)
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateReadyEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);
        var timeRange = VeaEventTestFactory.CreateTimeRange(VeaEventTestFactory.Parse(startRaw), VeaEventTestFactory.Parse(endRaw));

        // Act
        var result = veaEvent.UpdateTimeRange(timeRange, now);

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        Assert.Equal(timeRange, success.Value.TimeRange);
        Assert.Equal(EventStatus.Draft, success.Value.Status);
    }

    [Theory]
    [InlineData("2030-08-25 19:00", "2030-08-25 23:59")]
    [InlineData("2030-08-25 19:00", "2030-08-26 01:00")]
    public void UpdateTimeRange_WhenStartTimeIsInFuture_UpdatesTimeRange(
        string startRaw,
        string endRaw)
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);
        var timeRange = VeaEventTestFactory.CreateTimeRange(VeaEventTestFactory.Parse(startRaw), VeaEventTestFactory.Parse(endRaw));

        // Act
        var result = veaEvent.UpdateTimeRange(timeRange, now);

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        Assert.Equal(timeRange, success.Value.TimeRange);
    }

    [Theory]
    [InlineData("2030-08-25 08:00", "2030-08-25 18:00")]
    [InlineData("2030-08-25 13:00", "2030-08-25 23:00")]
    [InlineData("2030-08-25 15:00", "2030-08-26 01:00")]
    public void UpdateTimeRange_WhenDurationIsTenHoursOrLess_UpdatesTimeRange(
        string startRaw,
        string endRaw)
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);
        var timeRange = VeaEventTestFactory.CreateTimeRange(VeaEventTestFactory.Parse(startRaw), VeaEventTestFactory.Parse(endRaw));

        // Act
        var result = veaEvent.UpdateTimeRange(timeRange, now);

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        Assert.Equal(timeRange, success.Value.TimeRange);
    }

    [Theory]
    [InlineData("2030-08-26 19:00", "2030-08-25 01:00")]
    [InlineData("2030-08-26 19:00", "2030-08-25 23:59")]
    [InlineData("2030-08-27 12:00", "2030-08-25 16:30")]
    [InlineData("2030-08-01 08:00", "2030-07-31 12:15")]
    public void CreateTimeRange_WhenStartDateIsAfterEndDate_ReturnsFailure(
        string startRaw,
        string endRaw)
    {
        // Act
        var result = EventTimeRange.Create(VeaEventTestFactory.Parse(startRaw), VeaEventTestFactory.Parse(endRaw));

        // Assert
        var failure = Assert.IsType<Failure<EventTimeRange>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventTimeRange.EndMustBeAfterStart);
    }

    [Theory]
    [InlineData("2030-08-26 19:00", "2030-08-26 14:00")]
    [InlineData("2030-08-26 16:00", "2030-08-26 00:00")]
    [InlineData("2030-08-26 19:00", "2030-08-26 18:59")]
    [InlineData("2030-08-26 12:00", "2030-08-26 10:10")]
    [InlineData("2030-08-26 08:00", "2030-08-26 00:30")]
    public void CreateTimeRange_WhenStartTimeIsAfterEndTimeSameDay_ReturnsFailure(
        string startRaw,
        string endRaw)
    {
        // Act
        var result = EventTimeRange.Create(VeaEventTestFactory.Parse(startRaw), VeaEventTestFactory.Parse(endRaw));

        // Assert
        var failure = Assert.IsType<Failure<EventTimeRange>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventTimeRange.EndMustBeAfterStart);
    }

    [Theory]
    [InlineData("2030-08-26 14:00", "2030-08-26 14:50")]
    [InlineData("2030-08-26 18:00", "2030-08-26 18:59")]
    [InlineData("2030-08-26 12:00", "2030-08-26 12:30")]
    [InlineData("2030-08-26 08:00", "2030-08-26 08:00")]
    public void CreateTimeRange_WhenSameDayDurationIsTooShort_ReturnsFailure(
        string startRaw,
        string endRaw)
    {
        // Act
        var result = EventTimeRange.Create(VeaEventTestFactory.Parse(startRaw), VeaEventTestFactory.Parse(endRaw));

        // Assert
        var failure = Assert.IsType<Failure<EventTimeRange>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventTimeRange.TooShort);
    }

    [Theory]
    [InlineData("2030-08-25 23:30", "2030-08-26 00:15")]
    [InlineData("2030-08-30 23:01", "2030-08-31 00:00")]
    [InlineData("2030-08-30 23:59", "2030-08-31 00:01")]
    public void CreateTimeRange_WhenCrossDayDurationIsTooShort_ReturnsFailure(
        string startRaw,
        string endRaw)
    {
        // Act
        var result = EventTimeRange.Create(VeaEventTestFactory.Parse(startRaw), VeaEventTestFactory.Parse(endRaw));

        // Assert
        var failure = Assert.IsType<Failure<EventTimeRange>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventTimeRange.TooShort);
    }

    [Theory]
    [InlineData("2030-08-25 07:50", "2030-08-25 14:00")]
    [InlineData("2030-08-25 07:59", "2030-08-25 15:00")]
    [InlineData("2030-08-25 01:01", "2030-08-25 08:30")]
    [InlineData("2030-08-25 05:59", "2030-08-25 07:59")]
    [InlineData("2030-08-25 00:59", "2030-08-25 07:59")]
    public void CreateTimeRange_WhenStartTimeIsBeforeEight_ReturnsFailure(
        string startRaw,
        string endRaw)
    {
        // Act
        var result = EventTimeRange.Create(VeaEventTestFactory.Parse(startRaw), VeaEventTestFactory.Parse(endRaw));

        // Assert
        var failure = Assert.IsType<Failure<EventTimeRange>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventTimeRange.StartTooEarly);
    }

    [Theory]
    [InlineData("2030-08-24 23:50", "2030-08-25 01:01")]
    [InlineData("2030-08-24 22:00", "2030-08-25 07:59")]
    [InlineData("2030-08-30 23:00", "2030-08-31 02:30")]
    [InlineData("2030-08-24 23:50", "2030-08-25 01:01")]
    public void CreateTimeRange_WhenEndTimeIsAfterNextDayOneAM_ReturnsFailure(
        string startRaw,
        string endRaw)
    {
        // Act
        var result = EventTimeRange.Create(VeaEventTestFactory.Parse(startRaw), VeaEventTestFactory.Parse(endRaw));

        // Assert
        var failure = Assert.IsType<Failure<EventTimeRange>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventTimeRange.EndOutsideAllowedHours);
    }

    [Fact]
    public void UpdateTimeRange_WhenEventIsActive_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);
        var timeRange = VeaEventTestFactory.CreateTimeRange(
            new DateTime(2030, 08, 25, 19, 00, 00, DateTimeKind.Utc),
            new DateTime(2030, 08, 25, 23, 59, 00, DateTimeKind.Utc));

        // Act
        var result = veaEvent.UpdateTimeRange(timeRange, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.ActiveEventCannotBeModified);
        Assert.Equal(EventStatus.Active, veaEvent.Status);
    }

    [Fact]
    public void UpdateTimeRange_WhenEventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var cancelResult = veaEvent.Cancel();
        Assert.IsType<Success<VeaEvent>>(cancelResult);

        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);
        var timeRange = VeaEventTestFactory.CreateTimeRange(
            new DateTime(2030, 08, 25, 19, 00, 00, DateTimeKind.Utc),
            new DateTime(2030, 08, 25, 23, 59, 00, DateTimeKind.Utc));

        // Act
        var result = veaEvent.UpdateTimeRange(timeRange, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.CancelledEventCannotBeModified);
        Assert.Equal(EventStatus.Cancelled, veaEvent.Status);
    }

    [Theory]
    [InlineData("2030-08-30 08:00", "2030-08-30 18:01")]
    [InlineData("2030-08-30 14:59", "2030-08-31 01:00")]
    [InlineData("2030-08-30 14:00", "2030-08-31 00:01")]
    [InlineData("2030-08-30 14:00", "2030-08-31 18:30")]
    public void CreateTimeRange_WhenDurationIsTooLong_ReturnsFailure(
        string startRaw,
        string endRaw)
    {
        // Act
        var result = EventTimeRange.Create(VeaEventTestFactory.Parse(startRaw), VeaEventTestFactory.Parse(endRaw));

        // Assert
        var failure = Assert.IsType<Failure<EventTimeRange>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventTimeRange.TooLong);
    }

    [Fact]
    public void UpdateTimeRange_WhenStartTimeIsInThePast_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 25, 12, 00, 00, DateTimeKind.Utc);
        var timeRange = VeaEventTestFactory.CreateTimeRange(
            new DateTime(2030, 08, 25, 11, 00, 00, DateTimeKind.Utc),
            new DateTime(2030, 08, 25, 13, 00, 00, DateTimeKind.Utc));

        // Act
        var result = veaEvent.UpdateTimeRange(timeRange, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventTimeRangeMustBeInFuture);
    }

    [Theory]
    [InlineData("2030-08-31 00:30", "2030-08-31 08:30")]
    [InlineData("2030-08-30 23:59", "2030-08-31 08:01")]
    [InlineData("2030-08-31 01:00", "2030-08-31 08:00")]
    public void CreateTimeRange_WhenRangeSpansInvalidClosedHours_ReturnsFailure(
        string startRaw,
        string endRaw)
    {
        // Act
        var result = EventTimeRange.Create(VeaEventTestFactory.Parse(startRaw), VeaEventTestFactory.Parse(endRaw));

        // Assert
        var failure = Assert.IsType<Failure<EventTimeRange>>(result);
        Assert.True(
            failure.Errors.Contains(EventErrors.EventTimeRange.StartTooEarly) ||
            failure.Errors.Contains(EventErrors.EventTimeRange.EndOutsideAllowedHours) ||
            failure.Errors.Contains(EventErrors.EventTimeRange.TooLong));
    }
}