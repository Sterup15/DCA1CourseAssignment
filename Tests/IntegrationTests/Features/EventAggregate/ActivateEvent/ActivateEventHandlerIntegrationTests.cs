using IntegrationTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.ActivateEvent;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace IntegrationTests.Features.EventAggregate.ActivateEvent;

public class ActivateEventHandlerIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task HandleAsync_WhenEventIsReady_ActivatesEventAndPersists()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateReadyEvent();
        await DbContext.Events.AddAsync(veaEvent);
        await DbContext.SaveChangesAsync();

        var command = Assert.IsType<Success<ActivateEventCommand>>(
            ActivateEventCommand.Create(veaEvent.Id.ToString())).Value;
        var handler = new ActivateEventHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
        await using var fresh = CreateFreshContext();
        var savedEvent = await fresh.Events.FindAsync(veaEvent.Id);
        Assert.NotNull(savedEvent);
        Assert.Equal(EventStatusValue.Active, savedEvent.Status.Value);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<ActivateEventCommand>>(
            ActivateEventCommand.Create(EventId.New().ToString())).Value;
        var handler = new ActivateEventHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
