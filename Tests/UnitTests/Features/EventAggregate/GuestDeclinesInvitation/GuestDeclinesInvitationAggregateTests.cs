using UnitTests.Features.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Tools.OperationResult;

namespace UnitTests.Features.EventAggregate.GuestDeclinesInvitation;

public class GuestDeclinesInvitationAggregateTests
{
    [Fact]
    public void DeclineInvitation_WhenEventIsActiveAndGuestHasPendingInvitation_RemovesInvitation()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        var guest = GuestTestFactory.CreateGuest();

        var inviteResult = veaEvent.InviteGuest(guest.Id, ParticipationSource.Private);
        Assert.IsType<Result<VeaEvent>.Success>(inviteResult);

        Assert.Contains(
            veaEvent.Participations,
            p => p.GuestId == guest.Id && p.Status == ParticipationStatus.Invited);

        // Act
        var result = veaEvent.DeclineInvitation(guest.Id);

        // Assert
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        Assert.DoesNotContain(
            success.Value.Participations,
            p => p.GuestId == guest.Id);
    }

    [Fact]
    public void DeclineInvitation_WhenEventIsActiveAndGuestHasAcceptedInvitation_RemovesInvitation()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        var guest = GuestTestFactory.CreateGuest();

        var inviteResult = veaEvent.InviteGuest(guest.Id, ParticipationSource.Private);
        Assert.IsType<Result<VeaEvent>.Success>(inviteResult);

        var acceptResult = veaEvent.AcceptInvitation(guest.Id, VeaEventTestFactory.DefaultNow);
        Assert.IsType<Result<VeaEvent>.Success>(acceptResult);

        Assert.Contains(
            veaEvent.Participations,
            p => p.GuestId == guest.Id && p.Status == ParticipationStatus.Attending);

        // Act
        var result = veaEvent.DeclineInvitation(guest.Id);

        // Assert
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        Assert.DoesNotContain(
            success.Value.Participations,
            p => p.GuestId == guest.Id);
    }

    [Fact]
    public void DeclineInvitation_WhenInvitationIsNotFound_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        var guest = GuestTestFactory.CreateGuest();

        // Act
        var result = veaEvent.DeclineInvitation(guest.Id);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.ParticipationNotFound);

        Assert.Empty(veaEvent.Participations);
    }

    [Fact]
    public void DeclineInvitation_WhenEventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateReadyEvent();
        var guest = GuestTestFactory.CreateGuest();

        var inviteResult = veaEvent.InviteGuest(guest.Id, ParticipationSource.Private);
        Assert.IsType<Result<VeaEvent>.Success>(inviteResult);

        var cancelResult = veaEvent.Cancel();
        Assert.IsType<Result<VeaEvent>.Success>(cancelResult);

        // Act
        var result = veaEvent.DeclineInvitation(guest.Id);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.EventMustBeActiveToDeclineInvitation);

        Assert.Contains(
            veaEvent.Participations,
            p => p.GuestId == guest.Id && p.Status == ParticipationStatus.Invited);
    }
}