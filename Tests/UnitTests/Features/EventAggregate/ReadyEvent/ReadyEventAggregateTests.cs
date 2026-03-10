using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;

namespace UnitTests.Features.EventAggregate.ReadyEvent;

public class ReadyEventAggregateTests
{
    [Fact]
    public void MakeReady_WhenEventIsDraftAndRequiredDataIsValid_MakesEventReady()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        veaEvent.UpdateTitle(VeaEventTestFactory.CreateTitle("Scary Movie Night!"));
        veaEvent.UpdateDescription(VeaEventTestFactory.CreateDescription("A valid non-default description."));
        veaEvent.UpdateTimeRange(
            VeaEventTestFactory.CreateTimeRange(
                new DateTime(2030, 08, 25, 19, 00, 00, DateTimeKind.Utc),
                new DateTime(2030, 08, 25, 23, 00, 00, DateTimeKind.Utc)),
            now);

        // Visibility and GuestCapacity are already valid by default:
        // Visibility = Private, GuestCapacity = 5.

        // Act
        var result = veaEvent.MakeReady(now);

        // Assert
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        Assert.Equal(EventStatus.Ready, success.Value.Status);
    }

    [Fact]
    public void MakeReady_WhenTitleIsDefault_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        veaEvent.UpdateDescription(VeaEventTestFactory.CreateDescription("A valid non-default description."));
        veaEvent.UpdateTimeRange(
            VeaEventTestFactory.CreateTimeRange(
                new DateTime(2030, 08, 25, 19, 00, 00, DateTimeKind.Utc),
                new DateTime(2030, 08, 25, 23, 00, 00, DateTimeKind.Utc)),
            now);

        // Act
        var result = veaEvent.MakeReady(now);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.TitleMustNotBeDefault);
        Assert.Equal(EventStatus.Draft, veaEvent.Status);
    }

    [Fact]
    public void MakeReady_WhenDescriptionIsDefault_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        veaEvent.UpdateTitle(VeaEventTestFactory.CreateTitle("Scary Movie Night!"));
        veaEvent.UpdateTimeRange(
            VeaEventTestFactory.CreateTimeRange(
                new DateTime(2030, 08, 25, 19, 00, 00, DateTimeKind.Utc),
                new DateTime(2030, 08, 25, 23, 00, 00, DateTimeKind.Utc)),
            now);

        // Act
        var result = veaEvent.MakeReady(now);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.DescriptionMustNotBeDefault);
        Assert.Equal(EventStatus.Draft, veaEvent.Status);
    }

    [Fact]
    public void MakeReady_WhenTimeRangeIsNotSet_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        veaEvent.UpdateTitle(VeaEventTestFactory.CreateTitle("Scary Movie Night!"));
        veaEvent.UpdateDescription(VeaEventTestFactory.CreateDescription("A valid non-default description."));

        // Act
        var result = veaEvent.MakeReady(now);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.TimeRangeMustBeSet);
        Assert.Equal(EventStatus.Draft, veaEvent.Status);
    }

    [Fact]
    public void MakeReady_WhenMultipleRequiredValuesAreMissing_ReturnsAllRelevantFailures()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        // Title default, Description default, TimeRange null

        // Act
        var result = veaEvent.MakeReady(now);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.TitleMustNotBeDefault);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.DescriptionMustNotBeDefault);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.TimeRangeMustBeSet);
        Assert.Equal(EventStatus.Draft, veaEvent.Status);
    }

    [Fact]
    public void MakeReady_WhenEventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var cancelResult = veaEvent.Cancel();
        Assert.IsType<Result<VeaEvent>.Success>(cancelResult);

        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        // Act
        var result = veaEvent.MakeReady(now);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.CancelledEventCannotBeReadied);
        Assert.Equal(EventStatus.Cancelled, veaEvent.Status);
    }

    [Fact]
    public void MakeReady_WhenEventStartIsInThePast_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 25, 20, 00, 00, DateTimeKind.Utc);

        veaEvent.UpdateTitle(VeaEventTestFactory.CreateTitle("Scary Movie Night!"));
        veaEvent.UpdateDescription(VeaEventTestFactory.CreateDescription("A valid non-default description."));
        veaEvent.UpdateTimeRange(
            VeaEventTestFactory.CreateTimeRange(
                new DateTime(2030, 08, 25, 19, 00, 00, DateTimeKind.Utc),
                new DateTime(2030, 08, 25, 23, 00, 00, DateTimeKind.Utc)),
            new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc));

        // Act
        var result = veaEvent.MakeReady(now);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.PastEventsCannotBeReadied);
        Assert.Equal(EventStatus.Draft, veaEvent.Status);
    }

    [Fact]
    public void MakeReady_WhenTitleIsDefault_ReturnsSpecificFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        veaEvent.UpdateDescription(VeaEventTestFactory.CreateDescription("A valid non-default description."));
        veaEvent.UpdateTimeRange(
            VeaEventTestFactory.CreateTimeRange(
                new DateTime(2030, 08, 25, 19, 00, 00, DateTimeKind.Utc),
                new DateTime(2030, 08, 25, 23, 00, 00, DateTimeKind.Utc)),
            now);

        // Act
        var result = veaEvent.MakeReady(now);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.TitleMustNotBeDefault);
    }
}