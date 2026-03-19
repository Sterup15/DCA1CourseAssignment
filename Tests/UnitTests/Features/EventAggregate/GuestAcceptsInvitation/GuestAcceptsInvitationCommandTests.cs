using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.GuestAcceptsInvitation;

public class GuestAcceptsInvitationCommandTests
{
    [Fact]
    public void Create_WithValidIds_ReturnsSuccess()
    {
        // Arrange
        var eventId = EventId.New().ToString();
        var guestId = GuestId.New().ToString();

        // Act
        var result = GuestAcceptsInvitationCommand.Create(eventId, guestId);

        // Assert
        Assert.IsType<Success<GuestAcceptsInvitationCommand>>(result);
    }

    [Fact]
    public void Create_WithInvalidGuestId_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.New().ToString();

        // Act
        var result = GuestAcceptsInvitationCommand.Create(eventId, "not-a-guid");

        // Assert
        var failure = Assert.IsType<Failure<GuestAcceptsInvitationCommand>>(result);
        Assert.Contains(failure.Errors, e => e == GuestErrors.GuestId.InvalidFormat);
    }
}
