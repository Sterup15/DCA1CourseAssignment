using IntegrationTests.Common;
using Microsoft.EntityFrameworkCore;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.InviteGuest;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace IntegrationTests.Features.EventAggregate.InviteGuest;

public class InviteGuestHandlerIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task HandleAsync_WhenEventIsReady_InvitesGuestAndPersists()
    {
        // Arrange
        var guest = GuestTestFactory.CreateGuest();
        await DbContext.Guests.AddAsync(guest);
        var veaEvent = VeaEventTestFactory.CreateReadyEvent();
        await DbContext.Events.AddAsync(veaEvent);
        await DbContext.SaveChangesAsync();

        var command = Assert.IsType<Success<InviteGuestCommand>>(
            InviteGuestCommand.Create(veaEvent.Id.ToString(), guest.Id.ToString(), ParticipationSourceValue.Public)).Value;
        var handler = new InviteGuestHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
        await using var fresh = CreateFreshContext();
        var participation = await fresh.Set<Participation>()
            .FirstOrDefaultAsync(p => p.EventId == veaEvent.Id && p.GuestId == guest.Id);
        Assert.NotNull(participation);
        Assert.Equal(ParticipationStatusValue.Invited, participation.Status.Value);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<InviteGuestCommand>>(
            InviteGuestCommand.Create(EventId.New().ToString(), GuestId.New().ToString(), ParticipationSourceValue.Public)).Value;
        var handler = new InviteGuestHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
