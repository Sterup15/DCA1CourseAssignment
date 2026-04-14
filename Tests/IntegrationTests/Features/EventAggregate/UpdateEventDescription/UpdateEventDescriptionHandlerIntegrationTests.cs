using IntegrationTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.UpdateEventDescription;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace IntegrationTests.Features.EventAggregate.UpdateEventDescription;

public class UpdateEventDescriptionHandlerIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task HandleAsync_WhenEventExists_UpdatesDescriptionAndPersists()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        await DbContext.Events.AddAsync(veaEvent);
        await DbContext.SaveChangesAsync();

        var command = Assert.IsType<Success<UpdateEventDescriptionCommand>>(
            UpdateEventDescriptionCommand.Create(veaEvent.Id.ToString(), "A valid description for the event")).Value;
        var handler = new UpdateEventDescriptionHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
        await using var fresh = CreateFreshContext();
        var savedEvent = await fresh.Events.FindAsync(veaEvent.Id);
        Assert.Equal("A valid description for the event", savedEvent!.Description.Value);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<UpdateEventDescriptionCommand>>(
            UpdateEventDescriptionCommand.Create(EventId.New().ToString(), "A valid description")).Value;
        var handler = new UpdateEventDescriptionHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
