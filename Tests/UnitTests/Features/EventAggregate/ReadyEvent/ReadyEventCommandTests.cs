using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.ReadyEvent;

public class ReadyEventCommandTests
{
    [Fact]
    public void Create_WithValidEventId_ReturnsSuccess()
    {
        // Arrange
        var eventId = EventId.New().ToString();

        // Act
        var result = ReadyEventCommand.Create(eventId);

        // Assert
        Assert.IsType<Success<ReadyEventCommand>>(result);
    }

    [Fact]
    public void Create_WithInvalidEventId_ReturnsFailure()
    {
        // Act
        var result = ReadyEventCommand.Create("not-a-guid");

        // Assert
        var failure = Assert.IsType<Failure<ReadyEventCommand>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventId.InvalidFormat);
    }
}
