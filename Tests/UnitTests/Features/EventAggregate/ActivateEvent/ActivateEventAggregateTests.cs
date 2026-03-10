using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;

namespace UnitTests.Features.EventAggregate.ActivateEvent;

public class ActivateEventAggregateTests
{
    [Fact]
    public void MakeActive_WhenEventIsDraftAndRequiredDataIsValid_MakesEventReadyThenActive()
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

        // Visibility and GuestCapacity are already valid by default.

        // Act
        var result = veaEvent.MakeActive(now);

        // Assert
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        Assert.Equal(EventStatus.Active, success.Value.Status);
    }

    [Fact]
    public void MakeActive_WhenEventIsReady_MakesEventActive()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateReadyEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        // Act
        var result = veaEvent.MakeActive(now);

        // Assert
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        Assert.Equal(EventStatus.Active, success.Value.Status);
    }

    [Fact]
    public void MakeActive_WhenEventIsAlreadyActive_RemainsActive()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        // Act
        var result = veaEvent.MakeActive(now);

        // Assert
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        Assert.Equal(EventStatus.Active, success.Value.Status);
    }

    [Fact]
    public void MakeActive_WhenEventIsDraftAndTitleIsDefault_ReturnsFailure()
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
        var result = veaEvent.MakeActive(now);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.TitleMustNotBeDefault);
        Assert.Equal(EventStatus.Draft, veaEvent.Status);
    }

    [Fact]
    public void MakeActive_WhenEventIsDraftAndDescriptionIsDefault_ReturnsFailure()
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
        var result = veaEvent.MakeActive(now);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.DescriptionMustNotBeDefault);
        Assert.Equal(EventStatus.Draft, veaEvent.Status);
    }

    [Fact]
    public void MakeActive_WhenEventIsDraftAndTimeRangeIsNotSet_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        veaEvent.UpdateTitle(VeaEventTestFactory.CreateTitle("Scary Movie Night!"));
        veaEvent.UpdateDescription(VeaEventTestFactory.CreateDescription("A valid non-default description."));

        // Act
        var result = veaEvent.MakeActive(now);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.TimeRangeMustBeSet);
        Assert.Equal(EventStatus.Draft, veaEvent.Status);
    }

    [Fact]
    public void MakeActive_WhenEventIsDraftAndMultipleRequiredValuesAreMissing_ReturnsAllRelevantFailures()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        // Act
        var result = veaEvent.MakeActive(now);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.TitleMustNotBeDefault);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.DescriptionMustNotBeDefault);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.TimeRangeMustBeSet);
        Assert.Equal(EventStatus.Draft, veaEvent.Status);
    }

    [Fact]
    public void MakeActive_WhenEventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var cancelResult = veaEvent.Cancel();
        Assert.IsType<Result<VeaEvent>.Success>(cancelResult);

        var now = new DateTime(2030, 08, 24, 12, 00, 00, DateTimeKind.Utc);

        // Act
        var result = veaEvent.MakeActive(now);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.CancelledEventCannotBeActivated);
        Assert.Equal(EventStatus.Cancelled, veaEvent.Status);
    }
}