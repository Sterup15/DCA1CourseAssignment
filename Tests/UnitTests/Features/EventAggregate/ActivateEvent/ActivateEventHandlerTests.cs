using UnitTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.ActivateEvent;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.ActivateEvent;

public class ActivateEventHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenEventIsReady_ReturnsSuccess()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateReadyEvent();
        var command = Assert.IsType<Success<ActivateEventCommand>>(
            ActivateEventCommand.Create(veaEvent.Id.ToString())).Value;
        var handler = new ActivateEventHandler(new FakeVeaEventRepository(veaEvent), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<ActivateEventCommand>>(
            ActivateEventCommand.Create(EventId.New().ToString())).Value;
        var handler = new ActivateEventHandler(new FakeVeaEventRepository(null), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
