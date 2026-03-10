using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;

namespace UnitTests.Features.EventAggregate.UpdateEventTitle;

public class UpdateEventTitleAggregateTests
{
    [Theory]
    [InlineData("Scary Movie Night!")]
    [InlineData("Graduation Gala")]
    [InlineData("VIA Hackathon")]
    public void UpdateTitle_WhenEventIsDraft_UpdatesTitle(string newTitleRaw)
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var title = VeaEventTestFactory.CreateTitle(newTitleRaw);

        // Act
        var result = veaEvent.UpdateTitle(title);

        // Assert
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        Assert.Equal(newTitleRaw, success.Value.Title.Value);
        Assert.Equal(EventStatus.Draft, success.Value.Status);
    }

    [Theory]
    [InlineData("Scary Movie Night!")]
    [InlineData("Graduation Gala")]
    [InlineData("VIA Hackathon")]
    public void UpdateTitle_WhenEventIsReady_UpdatesTitleAndResetsStatusToDraft(string newTitleRaw)
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateReadyEvent();
        var title = VeaEventTestFactory.CreateTitle(newTitleRaw);

        // Act
        var result = veaEvent.UpdateTitle(title);

        // Assert
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        Assert.Equal(newTitleRaw, success.Value.Title.Value);
        Assert.Equal(EventStatus.Draft, success.Value.Status);
    }

    [Fact]
    public void CreateTitle_WhenTitleIsEmpty_ReturnsFailure()
    {
        // Act
        var result = EventTitle.Create(string.Empty);

        // Assert
        var failure = Assert.IsType<Result<EventTitle>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventTitle.Empty);
    }

    [Theory]
    [InlineData("XY")]
    [InlineData("a")]
    public void CreateTitle_WhenTitleIsTooShort_ReturnsFailure(string rawTitle)
    {
        // Act
        var result = EventTitle.Create(rawTitle);

        // Assert
        var failure = Assert.IsType<Result<EventTitle>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventTitle.TooShort);
    }

    [Fact]
    public void CreateTitle_WhenTitleIsTooLong_ReturnsFailure()
    {
        // Arrange
        var rawTitle = new string('A', 76);

        // Act
        var result = EventTitle.Create(rawTitle);

        // Assert
        var failure = Assert.IsType<Result<EventTitle>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventTitle.TooLong);
    }

    [Fact]
    public void CreateTitle_WhenTitleIsNull_ReturnsFailure()
    {
        // Act
        var result = EventTitle.Create(null);

        // Assert
        var failure = Assert.IsType<Result<EventTitle>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventTitle.Empty);
    }

    [Fact]
    public void UpdateTitle_WhenEventIsActive_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        var title = VeaEventTestFactory.CreateTitle("Scary Movie Night!");

        // Act
        var result = veaEvent.UpdateTitle(title);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.ActiveEventCannotBeModified);
        Assert.Equal(EventStatus.Active, veaEvent.Status);
    }

    [Fact]
    public void UpdateTitle_WhenEventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var cancelResult = veaEvent.Cancel();
        Assert.IsType<Result<VeaEvent>.Success>(cancelResult);

        var title = VeaEventTestFactory.CreateTitle("Scary Movie Night!");

        // Act
        var result = veaEvent.UpdateTitle(title);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.CancelledEventCannotBeModified);
        Assert.Equal(EventStatus.Cancelled, veaEvent.Status);
    }
}