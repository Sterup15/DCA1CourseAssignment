using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.GuestDeclinesInvitation;

public class GuestDeclinesInvitationCommandTests
{
    [Fact]
    public void Create_WithValidIds_ReturnsSuccess()
    {
        // Arrange
        var eventId = EventId.New().ToString();
        var guestId = GuestId.New().ToString();

        // Act
        var result = GuestDeclinesInvitationCommand.Create(eventId, guestId);

        // Assert
        Assert.IsType<Success<GuestDeclinesInvitationCommand>>(result);
    }

    [Fact]
    public void Create_WithInvalidEventId_ReturnsFailure()
    {
        // Arrange
        var guestId = GuestId.New().ToString();

        // Act
        var result = GuestDeclinesInvitationCommand.Create("not-a-guid", guestId);

        // Assert
        var failure = Assert.IsType<Failure<GuestDeclinesInvitationCommand>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventId.InvalidFormat);
    }
}
