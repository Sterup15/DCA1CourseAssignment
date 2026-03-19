using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.MakeEventPrivate;

public class MakeEventPrivateCommandTests
{
    [Fact]
    public void Create_WithValidEventId_ReturnsSuccess()
    {
        // Arrange
        var eventId = EventId.New().ToString();

        // Act
        var result = MakeEventPrivateCommand.Create(eventId);

        // Assert
        Assert.IsType<Success<MakeEventPrivateCommand>>(result);
    }

    [Fact]
    public void Create_WithInvalidEventId_ReturnsFailure()
    {
        // Act
        var result = MakeEventPrivateCommand.Create("not-a-guid");

        // Assert
        var failure = Assert.IsType<Failure<MakeEventPrivateCommand>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventId.InvalidFormat);
    }
}
