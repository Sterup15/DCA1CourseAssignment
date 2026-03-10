using UnitTests.Features.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Tools.OperationResult;

namespace UnitTests.Features.EventAggregate.InviteGuest;

public class InviteGuestAggregateTests
{
    [Theory]
    [InlineData(EventStatusValue.Ready)]
    [InlineData(EventStatusValue.Active)]
    public void InviteGuest_WhenEventIsReadyOrActive_RegistersPendingInvitation(EventStatusValue status)
    {
        // Arrange
        var veaEvent = status switch
        {
            EventStatusValue.Ready => VeaEventTestFactory.CreateReadyEvent(),
            EventStatusValue.Active => VeaEventTestFactory.CreateActiveEvent(),
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };

        var guest = GuestTestFactory.CreateGuest();

        // Act
        var result = veaEvent.InviteGuest(guest.Id, ParticipationSource.Private);

        // Assert
        var success = Assert.IsType<Result<VeaEvent>.Success>(result);
        var updatedEvent = success.Value;

        Assert.Single(updatedEvent.Participations);

        var participation = updatedEvent.Participations.Single();
        Assert.Equal(guest.Id, participation.GuestId);
        Assert.Equal(updatedEvent.Id, participation.EventId);
        Assert.Equal(ParticipationStatus.Invited, participation.Status);
        Assert.Equal(ParticipationSource.Private, participation.Source);
    }

    [Theory]
    [InlineData(EventStatusValue.Draft)]
    [InlineData(EventStatusValue.Cancelled)]
    public void InviteGuest_WhenEventIsDraftOrCancelled_ReturnsFailure(EventStatusValue status)
    {
        // Arrange
        var veaEvent = status switch
        {
            EventStatusValue.Draft => VeaEventTestFactory.CreateEvent(),
            EventStatusValue.Cancelled => VeaEventTestFactory.CreateCancelledEvent(),
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };

        var guest = GuestTestFactory.CreateGuest();

        // Act
        var result = veaEvent.InviteGuest(guest.Id, ParticipationSource.Private);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.GuestsCanOnlyBeInvitedToReadyOrActiveEvents);

        Assert.Empty(veaEvent.Participations);
    }

    [Fact]
    public void InviteGuest_WhenEventIsFull_ReturnsFailure()
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

        Assert.IsType<Result<VeaEvent>.Success>(veaEvent.ParticipateAsGuest(firstGuest.Id, now));
        Assert.IsType<Result<VeaEvent>.Success>(veaEvent.ParticipateAsGuest(secondGuest.Id, now));
        Assert.IsType<Result<VeaEvent>.Success>(veaEvent.ParticipateAsGuest(thirdGuest.Id, now));
        Assert.IsType<Result<VeaEvent>.Success>(veaEvent.ParticipateAsGuest(fourthGuest.Id, now));
        Assert.IsType<Result<VeaEvent>.Success>(veaEvent.ParticipateAsGuest(fifthGuest.Id, now));

        Assert.Equal(5, veaEvent.Participations.Count);

        // Act
        var result = veaEvent.InviteGuest(invitedGuest.Id, ParticipationSource.Private);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.CannotInviteToFullEvent);

        Assert.Equal(5, veaEvent.Participations.Count);
    }

    [Fact]
    public void InviteGuest_WhenGuestIsAlreadyInvited_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateReadyEvent();
        var guest = GuestTestFactory.CreateGuest();

        var firstInviteResult = veaEvent.InviteGuest(guest.Id, ParticipationSource.Private);
        Assert.IsType<Result<VeaEvent>.Success>(firstInviteResult);

        // Act
        var result = veaEvent.InviteGuest(guest.Id, ParticipationSource.Private);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.GuestAlreadyInvitedOrAttending);

        Assert.Single(veaEvent.Participations);
    }

    [Fact]
    public void InviteGuest_WhenGuestIsAlreadyParticipating_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        veaEvent.SetVisibilityToPublic();

        var guest = GuestTestFactory.CreateGuest();
        var now = VeaEventTestFactory.DefaultNow;

        var participateResult = veaEvent.ParticipateAsGuest(guest.Id, now);
        Assert.IsType<Result<VeaEvent>.Success>(participateResult);

        // Act
        var result = veaEvent.InviteGuest(guest.Id, ParticipationSource.Private);

        // Assert
        var failure = Assert.IsType<Result<VeaEvent>.Failure>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.GuestAlreadyInvitedOrAttending);

        Assert.Single(veaEvent.Participations);
    }

    
}