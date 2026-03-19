using UnitTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.SetEventGuestCapacity;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.SetEventGuestCapacity;

public class SetEventGuestCapacityHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenEventExists_ReturnsSuccess()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var command = Assert.IsType<Success<SetEventGuestCapacityCommand>>(
            SetEventGuestCapacityCommand.Create(veaEvent.Id.ToString(), 25)).Value;
        var handler = new SetEventGuestCapacityHandler(new FakeVeaEventRepository(veaEvent), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<SetEventGuestCapacityCommand>>(
            SetEventGuestCapacityCommand.Create(EventId.New().ToString(), 25)).Value;
        var handler = new SetEventGuestCapacityHandler(new FakeVeaEventRepository(null), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
