using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.SetEventGuestCapacity;

public class SetEventGuestCapacityCommandTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsSuccess()
    {
        // Arrange
        var eventId = EventId.New().ToString();

        // Act
        var result = SetEventGuestCapacityCommand.Create(eventId, 25);

        // Assert
        Assert.IsType<Success<SetEventGuestCapacityCommand>>(result);
    }

    [Fact]
    public void Create_WithCapacityBelowMinimum_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.New().ToString();

        // Act
        var result = SetEventGuestCapacityCommand.Create(eventId, 3);

        // Assert
        var failure = Assert.IsType<Failure<SetEventGuestCapacityCommand>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventGuestCapacity.TooSmall);
    }
}
