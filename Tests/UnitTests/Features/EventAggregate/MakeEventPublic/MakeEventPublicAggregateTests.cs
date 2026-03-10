using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;

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
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        Assert.Equal(EventVisibility.Public, success.Value.Visibility);
        Assert.Equal(originalStatus, success.Value.Status);
    }

    [Fact]
    public void SetVisibilityToPublic_WhenEventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var cancelResult = veaEvent.Cancel();
        Assert.IsType<Result<VeaEvent>.Success>(cancelResult);

        // Act
        var result = veaEvent.SetVisibilityToPublic();

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.CancelledEventCannotBeModified);
        Assert.Equal(EventStatus.Cancelled, veaEvent.Status);
        Assert.Equal(EventVisibility.Private, veaEvent.Visibility);
    }
}