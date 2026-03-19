using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.MakeEventPrivate;

public class MakeEventPrivateAggregateTests
{
    [Theory]
    [InlineData(EventStatusValue.Draft)]
    [InlineData(EventStatusValue.Ready)]
    public void SetVisibilityToPrivate_WhenEventIsAlreadyPrivateAndStatusIsDraftOrReady_KeepsVisibilityPrivateAndStatusUnchanged(
        EventStatusValue initialStatus)
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEventWithStatus(initialStatus);
        var originalStatus = veaEvent.Status;

        Assert.Equal(EventVisibility.Private, veaEvent.Visibility);

        // Act
        var result = veaEvent.SetVisibilityToPrivate();

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        Assert.Equal(EventVisibility.Private, success.Value.Visibility);
        Assert.Equal(originalStatus, success.Value.Status);
    }

    [Theory]
    [InlineData(EventStatusValue.Draft)]
    [InlineData(EventStatusValue.Ready)]
    public void SetVisibilityToPrivate_WhenEventIsPublicAndStatusIsDraftOrReady_MakesEventPrivateAndKeepsStatus(
        EventStatusValue initialStatus)
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEventWithStatus(initialStatus);
        var makePublicResult = veaEvent.SetVisibilityToPublic();
        Assert.IsType<Success<VeaEvent>>(makePublicResult);
        Assert.Equal(EventVisibility.Public, veaEvent.Visibility);

        var originalStatus = veaEvent.Status;

        // Act
        var result = veaEvent.SetVisibilityToPrivate();

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        Assert.Equal(EventVisibility.Private, success.Value.Visibility);
        Assert.Equal(originalStatus, success.Value.Status);
    }

    [Fact]
    public void SetVisibilityToPrivate_WhenEventIsActive_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();

        // Act
        var result = veaEvent.SetVisibilityToPrivate();

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.ActiveEventCannotBeMadePrivate);
        Assert.Equal(EventStatus.Active, veaEvent.Status);
    }

    [Fact]
    public void SetVisibilityToPrivate_WhenEventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var cancelResult = veaEvent.Cancel();
        Assert.IsType<Success<VeaEvent>>(cancelResult);

        // Act
        var result = veaEvent.SetVisibilityToPrivate();

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.CancelledEventCannotBeModified);
        Assert.Equal(EventStatus.Cancelled, veaEvent.Status);
    }
}