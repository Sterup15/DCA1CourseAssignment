using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.MakePublic;

public class MakeEventPublicAggregateTests
{
    [Theory]
    [InlineData(EventStatusValue.Draft)]
    [InlineData(EventStatusValue.Ready)]
    [InlineData(EventStatusValue.Active)]
    public void SetVisibilityToPublic_WhenEventIsDraftReadyOrActive_MakesEventPublicAndKeepsStatusUnchanged(
        EventStatusValue initialStatus)
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEventWithStatus(initialStatus);
        var originalStatus = veaEvent.Status;

        // Act
        var result = veaEvent.SetVisibilityToPublic();

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        Assert.Equal(EventVisibility.Public, success.Value.Visibility);
        Assert.Equal(originalStatus, success.Value.Status);
    }

    [Fact]
    public void SetVisibilityToPublic_WhenEventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var cancelResult = veaEvent.Cancel();
        Assert.IsType<Success<VeaEvent>>(cancelResult);

        // Act
        var result = veaEvent.SetVisibilityToPublic();

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.CancelledEventCannotBeModified);
        Assert.Equal(EventStatus.Cancelled, veaEvent.Status);
        Assert.Equal(EventVisibility.Private, veaEvent.Visibility);
    }
}