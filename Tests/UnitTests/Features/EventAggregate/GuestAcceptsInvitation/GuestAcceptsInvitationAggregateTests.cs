using UnitTests.Features.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.GuestAcceptsInvitation;

public class GuestAcceptsInvitationAggregateTests
{
    [Fact]
    public void AcceptInvitation_WhenEventIsActiveAndInvitationExistsAndThereIsRoom_ChangesInvitationToAccepted()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        var guest = GuestTestFactory.CreateGuest();

        var inviteResult = veaEvent.InviteGuest(guest.Id, ParticipationSource.Private);
        Assert.IsType<Success<VeaEvent>>(inviteResult);

        var now = VeaEventTestFactory.DefaultNow;

        // Act
        var result = veaEvent.AcceptInvitation(guest.Id, now);

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        var participation = Assert.Single(success.Value.Participations);

        Assert.Equal(guest.Id, participation.GuestId);
        Assert.Equal(ParticipationStatus.Attending, participation.Status);
        Assert.Equal(ParticipationSource.Private, participation.Source);
    }

    [Fact]
    public void AcceptInvitation_WhenInvitationDoesNotExist_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        var guest = GuestTestFactory.CreateGuest();
        var now = VeaEventTestFactory.DefaultNow;

        // Act
        var result = veaEvent.AcceptInvitation(guest.Id, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.ParticipationNotFound);

        Assert.Empty(veaEvent.Participations);
    }

    [Fact]
    public void AcceptInvitation_WhenEventIsFull_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        veaEvent.SetVisibilityToPublic();

        var now = VeaEventTestFactory.DefaultNow;

        var firstGuest = GuestTestFactory.CreateGuest(email: "abc@via.dk");
        var secondGuest = GuestTestFactory.CreateGuest(email: "abcd@via.dk");
        var thirdGuest = GuestTestFactory.CreateGuest(email: "123456@via.dk");
        var fourthGuest = GuestTestFactory.CreateGuest(email: "wxyz@via.dk");
        var fifthGuest = GuestTestFactory.CreateGuest(email: "lmno@via.dk");
        var invitedGuest = GuestTestFactory.CreateGuest(email: "pqrs@via.dk");

        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(firstGuest.Id, now));
        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(secondGuest.Id, now));
        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(thirdGuest.Id, now));
        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(fourthGuest.Id, now));
        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(fifthGuest.Id, now));

        var inviteResult = veaEvent.InviteGuest(invitedGuest.Id, ParticipationSource.Private);

        // Since InviteGuest in your aggregate already blocks inviting when full,
        // we need a pending invitation before the event becomes full.
        // So recreate setup accordingly.
        veaEvent = VeaEventTestFactory.CreateActiveEvent();
        veaEvent.SetVisibilityToPublic();

        invitedGuest = GuestTestFactory.CreateGuest(email: "pqrs@via.dk");
        Assert.IsType<Success<VeaEvent>>(veaEvent.InviteGuest(invitedGuest.Id, ParticipationSource.Private));

        firstGuest = GuestTestFactory.CreateGuest(email: "abc@via.dk");
        secondGuest = GuestTestFactory.CreateGuest(email: "abcd@via.dk");
        thirdGuest = GuestTestFactory.CreateGuest(email: "123456@via.dk");
        fourthGuest = GuestTestFactory.CreateGuest(email: "wxyz@via.dk");
        fifthGuest = GuestTestFactory.CreateGuest(email: "lmno@via.dk");

        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(firstGuest.Id, now));
        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(secondGuest.Id, now));
        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(thirdGuest.Id, now));
        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(fourthGuest.Id, now));
        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(fifthGuest.Id, now));

        // Act
        var result = veaEvent.AcceptInvitation(invitedGuest.Id, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.EventCapacityReached);

        Assert.Contains(
            veaEvent.Participations,
            p => p.GuestId == invitedGuest.Id && p.Status == ParticipationStatus.Invited);
    }

    [Fact]
    public void AcceptInvitation_WhenEventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateReadyEvent();
        var guest = GuestTestFactory.CreateGuest();

        var inviteResult = veaEvent.InviteGuest(guest.Id, ParticipationSource.Private);
        Assert.IsType<Success<VeaEvent>>(inviteResult);

        var cancelResult = veaEvent.Cancel();
        Assert.IsType<Success<VeaEvent>>(cancelResult);

        var now = VeaEventTestFactory.DefaultNow;

        // Act
        var result = veaEvent.AcceptInvitation(guest.Id, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.EventMustBeActiveToAcceptInvitation);

        Assert.Contains(
            veaEvent.Participations,
            p => p.GuestId == guest.Id && p.Status == ParticipationStatus.Invited);
    }

    [Fact]
    public void AcceptInvitation_WhenEventIsReady_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateReadyEvent();
        var guest = GuestTestFactory.CreateGuest();

        var inviteResult = veaEvent.InviteGuest(guest.Id, ParticipationSource.Private);
        Assert.IsType<Success<VeaEvent>>(inviteResult);

        var now = VeaEventTestFactory.DefaultNow;

        // Act
        var result = veaEvent.AcceptInvitation(guest.Id, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.EventMustBeActiveToAcceptInvitation);

        Assert.Contains(
            veaEvent.Participations,
            p => p.GuestId == guest.Id && p.Status == ParticipationStatus.Invited);
    }

    [Fact]
    public void AcceptInvitation_WhenEventIsInThePast_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        var guest = GuestTestFactory.CreateGuest();

        var inviteResult = veaEvent.InviteGuest(guest.Id, ParticipationSource.Private);
        Assert.IsType<Success<VeaEvent>>(inviteResult);

        var afterStart = new DateTime(2030, 08, 25, 13, 30, 00, DateTimeKind.Utc);

        // Act
        var result = veaEvent.AcceptInvitation(guest.Id, afterStart);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.OnlyFutureEventsCanBeParticipated);

        Assert.Contains(
            veaEvent.Participations,
            p => p.GuestId == guest.Id && p.Status == ParticipationStatus.Invited);
    }
}