using UnitTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.CancelEventParticipation;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.CancelEventParticipation;

public class CancelEventParticipationHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenParticipationExists_ReturnsSuccess()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        veaEvent.SetVisibilityToPublic();
        var guestId = GuestId.New();
        veaEvent.ParticipateAsGuest(guestId, VeaEventTestFactory.DefaultNow);

        var db = new FakeDbContext();
        db.Events.Add(veaEvent);
        await db.SaveChangesAsync();
        var command = Assert.IsType<Success<CancelEventParticipationCommand>>(
            CancelEventParticipationCommand.Create(veaEvent.Id.ToString(), guestId.ToString())).Value;
        var handler = new CancelEventParticipationHandler(new FakeVeaEventRepository(db), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<CancelEventParticipationCommand>>(
            CancelEventParticipationCommand.Create(EventId.New().ToString(), GuestId.New().ToString())).Value;
        var handler = new CancelEventParticipationHandler(new FakeVeaEventRepository(new FakeDbContext()), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
