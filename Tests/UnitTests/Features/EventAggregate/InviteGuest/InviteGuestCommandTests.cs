using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.InviteGuest;

public class InviteGuestCommandTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsSuccess()
    {
        // Arrange
        var eventId = EventId.New().ToString();
        var guestId = GuestId.New().ToString();

        // Act
        var result = InviteGuestCommand.Create(eventId, guestId, ParticipationSourceValue.Public);

        // Assert
        Assert.IsType<Success<InviteGuestCommand>>(result);
    }

    [Fact]
    public void Create_WithInvalidEventId_ReturnsFailure()
    {
        // Arrange
        var guestId = GuestId.New().ToString();

        // Act
        var result = InviteGuestCommand.Create("not-a-guid", guestId, ParticipationSourceValue.Public);

        // Assert
        var failure = Assert.IsType<Failure<InviteGuestCommand>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventId.InvalidFormat);
    }
}
