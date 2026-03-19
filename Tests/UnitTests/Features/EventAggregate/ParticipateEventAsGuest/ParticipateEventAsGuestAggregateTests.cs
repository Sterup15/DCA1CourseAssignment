using UnitTests.Features.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.ParticipateEventAsGuest;

public class ParticipateEventAsGuestAggregateTests
{
    [Fact]
    public void ParticipateAsGuest_WhenEventIsActivePublicAndHasRoom_AddsGuestParticipation()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        veaEvent.SetVisibilityToPublic();

        var guest = GuestTestFactory.CreateGuest();
        var now = VeaEventTestFactory.DefaultNow;

        // Act
        var result = veaEvent.ParticipateAsGuest(guest.Id, now);

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        var updatedEvent = success.Value;

        Assert.Single(updatedEvent.Participations);

        var participation = updatedEvent.Participations.Single();
        Assert.Equal(guest.Id, participation.GuestId);
        Assert.Equal(updatedEvent.Id, participation.EventId);
        Assert.Equal(ParticipationStatus.Attending, participation.Status);
        Assert.Equal(ParticipationSource.Public, participation.Source);
    }

    [Theory]
    [InlineData(EventStatusValue.Draft)]
    [InlineData(EventStatusValue.Ready)]
    public void ParticipateAsGuest_WhenEventIsNotActive_ReturnsFailure(EventStatusValue status)
    {
        // Arrange
        var veaEvent = status switch
        {
            EventStatusValue.Draft => VeaEventTestFactory.CreateEvent(),
            EventStatusValue.Ready => VeaEventTestFactory.CreateReadyEvent(),
            _ => throw new ArgumentOutOfRangeException(nameof(status))
        };

        veaEvent.SetVisibilityToPublic();
        var guest = GuestTestFactory.CreateGuest();
        var now = VeaEventTestFactory.DefaultNow;

        // Act
        var result = veaEvent.ParticipateAsGuest(guest.Id, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.OnlyActiveEventsCanBeJoined);
        Assert.Empty(veaEvent.Participations);
    }

    [Fact]
    public void ParticipateAsGuest_WhenEventIsCancelled_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        veaEvent.SetVisibilityToPublic();
        var cancelResult = veaEvent.Cancel();
        Assert.IsType<Success<VeaEvent>>(cancelResult);

        var guest = GuestTestFactory.CreateGuest();
        var now = VeaEventTestFactory.DefaultNow;

        // Act
        var result = veaEvent.ParticipateAsGuest(guest.Id, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.OnlyActiveEventsCanBeJoined);
        Assert.Empty(veaEvent.Participations);
    }

    [Fact]
    public void ParticipateAsGuest_WhenEventHasNoMoreRoom_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        veaEvent.SetVisibilityToPublic();

        var firstGuest = GuestTestFactory.CreateGuest(email: "abc@via.dk");
        var secondGuest = GuestTestFactory.CreateGuest(email: "abcd@via.dk");
        var thirdGuest = GuestTestFactory.CreateGuest(email: "123456@via.dk");
        var fourthGuest = GuestTestFactory.CreateGuest(email: "wxyz@via.dk");
        var fifthGuest = GuestTestFactory.CreateGuest(email: "lmno@via.dk");
        var sixthGuest = GuestTestFactory.CreateGuest(email: "pqrs@via.dk");

        var now = VeaEventTestFactory.DefaultNow;

        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(firstGuest.Id, now));
        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(secondGuest.Id, now));
        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(thirdGuest.Id, now));
        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(fourthGuest.Id, now));
        Assert.IsType<Success<VeaEvent>>(veaEvent.ParticipateAsGuest(fifthGuest.Id, now));

        // Act
        var result = veaEvent.ParticipateAsGuest(sixthGuest.Id, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.EventCapacityReached);
        Assert.Equal(5, veaEvent.Participations.Count);
    }

    [Fact]
    public void ParticipateAsGuest_WhenEventStartIsInPast_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        veaEvent.SetVisibilityToPublic();

        var guest = GuestTestFactory.CreateGuest();
        var now = new DateTime(2030, 08, 25, 13, 30, 00, DateTimeKind.Utc);

        // Act
        var result = veaEvent.ParticipateAsGuest(guest.Id, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.OnlyFutureEventsCanBeParticipated);
        Assert.Empty(veaEvent.Participations);
    }

    [Fact]
    public void ParticipateAsGuest_WhenEventIsPrivate_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        Assert.Equal(EventVisibility.Private, veaEvent.Visibility);

        var guest = GuestTestFactory.CreateGuest();
        var now = VeaEventTestFactory.DefaultNow;

        // Act
        var result = veaEvent.ParticipateAsGuest(guest.Id, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.OnlyPublicEventsCanBeParticipatedDirectly);
        Assert.Empty(veaEvent.Participations);
    }

    [Fact]
    public void ParticipateAsGuest_WhenGuestAlreadyParticipates_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        veaEvent.SetVisibilityToPublic();

        var guest = GuestTestFactory.CreateGuest();
        var now = VeaEventTestFactory.DefaultNow;

        var firstJoinResult = veaEvent.ParticipateAsGuest(guest.Id, now);
        Assert.IsType<Success<VeaEvent>>(firstJoinResult);

        // Act
        var result = veaEvent.ParticipateAsGuest(guest.Id, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.GuestAlreadyHasParticipation);
        Assert.Single(veaEvent.Participations);
    }
}