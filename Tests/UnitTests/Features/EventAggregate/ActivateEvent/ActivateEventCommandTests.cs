using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.ActivateEvent;

public class ActivateEventCommandTests
{
    [Fact]
    public void Create_WithValidEventId_ReturnsSuccess()
    {
        // Arrange
        var eventId = EventId.New().ToString();

        // Act
        var result = ActivateEventCommand.Create(eventId);

        // Assert
        Assert.IsType<Success<ActivateEventCommand>>(result);
    }

    [Fact]
    public void Create_WithInvalidEventId_ReturnsFailure()
    {
        // Act
        var result = ActivateEventCommand.Create("not-a-guid");

        // Assert
        var failure = Assert.IsType<Failure<ActivateEventCommand>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventId.InvalidFormat);
    }
}
