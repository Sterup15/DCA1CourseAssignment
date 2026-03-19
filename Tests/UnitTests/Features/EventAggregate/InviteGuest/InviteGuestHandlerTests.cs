using UnitTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.InviteGuest;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.InviteGuest;

public class InviteGuestHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenEventIsReady_ReturnsSuccess()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateReadyEvent();
        var guestId = GuestId.New();

        var command = Assert.IsType<Success<InviteGuestCommand>>(
            InviteGuestCommand.Create(veaEvent.Id.ToString(), guestId.ToString(), ParticipationSourceValue.Public)).Value;
        var handler = new InviteGuestHandler(new FakeVeaEventRepository(veaEvent), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<InviteGuestCommand>>(
            InviteGuestCommand.Create(EventId.New().ToString(), GuestId.New().ToString(), ParticipationSourceValue.Public)).Value;
        var handler = new InviteGuestHandler(new FakeVeaEventRepository(null), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
