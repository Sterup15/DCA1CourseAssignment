using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.MakeEventPublic;

public class MakeEventPublicCommandTests
{
    [Fact]
    public void Create_WithValidEventId_ReturnsSuccess()
    {
        // Arrange
        var eventId = EventId.New().ToString();

        // Act
        var result = MakeEventPublicCommand.Create(eventId);

        // Assert
        Assert.IsType<Success<MakeEventPublicCommand>>(result);
    }

    [Fact]
    public void Create_WithInvalidEventId_ReturnsFailure()
    {
        // Act
        var result = MakeEventPublicCommand.Create("not-a-guid");

        // Assert
        var failure = Assert.IsType<Failure<MakeEventPublicCommand>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventId.InvalidFormat);
    }
}
