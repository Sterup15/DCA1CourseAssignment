using UnitTests.Features.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.CancelEventParticipation;

public class CancelEventParticipationAggregateTests
{
    [Fact]
    public void CancelParticipation_WhenGuestIsParticipating_RemovesParticipation()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        veaEvent.SetVisibilityToPublic();

        var guest = GuestTestFactory.CreateGuest();
        var now = VeaEventTestFactory.DefaultNow;

        var joinResult = veaEvent.ParticipateAsGuest(guest.Id, now);
        Assert.IsType<Success<VeaEvent>>(joinResult);
        Assert.Single(veaEvent.Participations);

        // Act
        var result = veaEvent.CancelParticipation(guest.Id, now);

        // Assert
        var success = Assert.IsType<Success<VeaEvent>>(result);
        Assert.Empty(success.Value.Participations);
    }

    [Fact]
    public void CancelParticipation_WhenGuestIsNotParticipating_ReturnsFailureAndNothingChanges()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        veaEvent.SetVisibilityToPublic();

        var guest = GuestTestFactory.CreateGuest();
        var now = VeaEventTestFactory.DefaultNow;

        Assert.Empty(veaEvent.Participations);

        // Act
        var result = veaEvent.CancelParticipation(guest.Id, now);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.ParticipationNotFound);

        Assert.Empty(veaEvent.Participations);
    }

    [Fact]
    public void CancelParticipation_WhenEventIsActiveAndStartedAndGuestIsParticipating_ReturnsFailure()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateActiveEvent();
        veaEvent.SetVisibilityToPublic();

        var guest = GuestTestFactory.CreateGuest();
        var beforeStart = VeaEventTestFactory.DefaultNow;

        var joinResult = veaEvent.ParticipateAsGuest(guest.Id, beforeStart);
        Assert.IsType<Success<VeaEvent>>(joinResult);
        Assert.Single(veaEvent.Participations);

        var afterStart = new DateTime(2030, 08, 25, 13, 30, 00, DateTimeKind.Utc);

        // Act
        var result = veaEvent.CancelParticipation(guest.Id, afterStart);

        // Assert
        var failure = Assert.IsType<Failure<VeaEvent>>(result);
        Assert.Contains(
            failure.Errors,
            e => e == EventErrors.Participation.CannotCancelParticipationInPastOrOngoingEvents);

        Assert.Single(veaEvent.Participations);
    }
}