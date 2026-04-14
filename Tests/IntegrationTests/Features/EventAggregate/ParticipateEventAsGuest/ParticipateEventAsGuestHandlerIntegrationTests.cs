using IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.ParticipateEventAsGuest;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace IntegrationTests.Features.EventAggregate.ParticipateEventAsGuest;

public class ParticipateEventAsGuestHandlerIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task HandleAsync_WhenEventIsActiveAndPublic_AddsParticipationAndPersists()
    {
        // Arrange
        var guest = GuestTestFactory.CreateGuest();
        await DbContext.Guests.AddAsync(guest);
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        veaEvent.SetVisibilityToPublic();
        await DbContext.Events.AddAsync(veaEvent);
        await DbContext.SaveChangesAsync();

        var command = Assert.IsType<Success<ParticipateEventAsGuestCommand>>(
            ParticipateEventAsGuestCommand.Create(veaEvent.Id.ToString(), guest.Id.ToString())).Value;
        var handler = new ParticipateEventAsGuestHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
        await using var fresh = CreateFreshContext();
        var participation = await fresh.Set<Participation>()
            .FirstOrDefaultAsync(p => p.EventId == veaEvent.Id && p.GuestId == guest.Id);
        Assert.NotNull(participation);
        Assert.Equal(ParticipationStatusValue.Attending, participation.Status.Value);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<ParticipateEventAsGuestCommand>>(
            ParticipateEventAsGuestCommand.Create(EventId.New().ToString(), GuestId.New().ToString())).Value;
        var handler = new ParticipateEventAsGuestHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
