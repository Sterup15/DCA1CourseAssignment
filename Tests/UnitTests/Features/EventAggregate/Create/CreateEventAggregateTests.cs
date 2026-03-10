using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;

namespace UnitTests.Features.EventAggregate.Create;

public class CreateEventAggregateTests
{
    [Fact]
    public void Create_WhenCalled_CreatesEmptyEventWithIdStatusDraftAndGuestCapacity5()
    {
        // Act
        var result = VeaEvent.Create();

        // Assert
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        var veaEvent = success.Value;

        Assert.NotEqual(default, veaEvent.Id);
        Assert.NotEqual(Guid.Empty, veaEvent.Id.Value);
        Assert.Equal(EventStatus.Draft, veaEvent.Status);
        Assert.Equal(5, veaEvent.GuestCapacity.Value);
        Assert.Null(veaEvent.TimeRange);
        Assert.Null(veaEvent.Location);
        Assert.Empty(veaEvent.Participations);
    }

    [Fact]
    public void Create_WhenCalled_SetsTitleToWorkingTitle()
    {
        // Act
        var result = VeaEvent.Create();

        // Assert
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        var veaEvent = success.Value;

        Assert.Equal("Working Title", veaEvent.Title.Value);
    }

    [Fact]
    public void Create_WhenCalled_SetsDescriptionToEmptyString()
    {
        // Act
        var result = VeaEvent.Create();

        // Assert
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        var veaEvent = success.Value;

        Assert.Equal(string.Empty, veaEvent.Description.Value);
    }

    [Fact]
    public void Create_WhenCalled_SetsVisibilityToPrivate()
    {
        // Act
        var result = VeaEvent.Create();

        // Assert
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        var veaEvent = success.Value;

        Assert.Equal(EventVisibility.Private, veaEvent.Visibility);
    }
}