using IntegrationTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.SetEventGuestCapacity;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace IntegrationTests.Features.EventAggregate.SetEventGuestCapacity;

public class SetEventGuestCapacityHandlerIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task HandleAsync_WhenEventExists_SetsCapacityAndPersists()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        await DbContext.Events.AddAsync(veaEvent);
        await DbContext.SaveChangesAsync();

        var command = Assert.IsType<Success<SetEventGuestCapacityCommand>>(
            SetEventGuestCapacityCommand.Create(veaEvent.Id.ToString(), 50)).Value;
        var handler = new SetEventGuestCapacityHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
        await using var fresh = CreateFreshContext();
        var savedEvent = await fresh.Events.FindAsync(veaEvent.Id);
        Assert.Equal(50, savedEvent!.GuestCapacity.Value);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<SetEventGuestCapacityCommand>>(
            SetEventGuestCapacityCommand.Create(EventId.New().ToString(), 50)).Value;
        var handler = new SetEventGuestCapacityHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
