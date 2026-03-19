using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.UpdateEventDescription;

public class UpdateEventDescriptionAggregateTests
{
    [Fact]
    public void UpdateDescription_WhenEventIsDraft_UpdatesDescription()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var description = VeaEventTestFactory.CreateDescription(
            "Nullam tempor lacus nisl, eget tempus quam maximus malesuada. Morbi faucibus sed neque vitae euismod.");

        // Act
        var result = veaEvent.UpdateDescription(description);

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        Assert.Equal(description.Value, success.Value.Description.Value);
        Assert.Equal(EventStatus.Draft, success.Value.Status);
    }

    [Fact]
    public void UpdateDescription_WhenSetToEmpty_SetsEmptyDescription()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var description = VeaEventTestFactory.CreateDescription(string.Empty);

        // Act
        var result = veaEvent.UpdateDescription(description);

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        Assert.Equal(string.Empty, success.Value.Description.Value);
    }

    [Theory]
    [InlineData("Nullam tempor lacus nisl, eget tempus quam maximus malesuada.")]
    [InlineData("")]
    public void UpdateDescription_WhenEventIsReady_UpdatesDescriptionAndResetsStatusToDraft(string rawDescription)
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateReadyEvent();
        var description = VeaEventTestFactory.CreateDescription(rawDescription);

        // Act
        var result = veaEvent.UpdateDescription(description);

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        Assert.Equal(description.Value, success.Value.Description.Value);
        Assert.Equal(EventStatus.Draft, success.Value.Status);
    }

    [Fact]
    public void CreateDescription_WhenDescriptionIsTooLong_ReturnsFailure()
    {
        // Arrange
        var rawDescription = new string('A', 251);

        // Act
        var result = EventDescription.Create(rawDescription);

        // Assert
        var failure = Assert.IsType<Failure<EventDescription>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventDescription.TooLong);
    }

    [Fact]
    public void UpdateDescription_WhenEventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var cancelResult = veaEvent.Cancel();
        Assert.IsType<Success<VeaEvent>>(cancelResult);

        var description = VeaEventTestFactory.CreateDescription("Valid description");

        // Act
        var result = veaEvent.UpdateDescription(description);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.CancelledEventCannotBeModified);
        Assert.Equal(EventStatus.Cancelled, veaEvent.Status);
    }

    [Fact]
    public void UpdateDescription_WhenEventIsActive_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        var description = VeaEventTestFactory.CreateDescription("Valid description");

        // Act
        var result = veaEvent.UpdateDescription(description);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.ActiveEventCannotBeModified);
        Assert.Equal(EventStatus.Active, veaEvent.Status);
    }
}