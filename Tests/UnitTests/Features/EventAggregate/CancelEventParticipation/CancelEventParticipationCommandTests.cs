using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.CancelEventParticipation;

public class CancelEventParticipationCommandTests
{
    [Fact]
    public void Create_WithValidIds_ReturnsSuccess()
    {
        // Arrange
        var eventId = EventId.New().ToString();
        var guestId = GuestId.New().ToString();

        // Act
        var result = CancelEventParticipationCommand.Create(eventId, guestId);

        // Assert
        Assert.IsType<Success<CancelEventParticipationCommand>>(result);
    }

    [Fact]
    public void Create_WithInvalidEventId_ReturnsFailure()
    {
        // Arrange
        var guestId = GuestId.New().ToString();

        // Act
        var result = CancelEventParticipationCommand.Create("not-a-guid", guestId);

        // Assert
        var failure = Assert.IsType<Failure<CancelEventParticipationCommand>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventId.InvalidFormat);
    }
}
