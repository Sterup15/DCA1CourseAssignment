using UnitTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.GuestAcceptsInvitation;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.GuestAcceptsInvitation;

public class GuestAcceptsInvitationHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenGuestIsInvited_ReturnsSuccess()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        var guestId = GuestId.New();
        veaEvent.InviteGuest(guestId, ParticipationSource.Public);

        var command = Assert.IsType<Success<GuestAcceptsInvitationCommand>>(
            GuestAcceptsInvitationCommand.Create(veaEvent.Id.ToString(), guestId.ToString())).Value;
        var handler = new GuestAcceptsInvitationHandler(new FakeVeaEventRepository(veaEvent), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<GuestAcceptsInvitationCommand>>(
            GuestAcceptsInvitationCommand.Create(EventId.New().ToString(), GuestId.New().ToString())).Value;
        var handler = new GuestAcceptsInvitationHandler(new FakeVeaEventRepository(null), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
