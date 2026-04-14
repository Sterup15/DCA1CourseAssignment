using IntegrationTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.CreateEvent;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult.Result;

namespace IntegrationTests.Features.EventAggregate.CreateEvent;

public class CreateEventHandlerIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task HandleAsync_CreatesEventAndPersistsToDatabase()
    {
        // Arrange
        var command = new CreateEventCommand();
        var handler = new CreateEventHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var success = Assert.IsType<Success<EventId>>(result);
        await using var fresh = CreateFreshContext();
        var savedEvent = await fresh.Events.FindAsync(success.Value);
        Assert.NotNull(savedEvent);
    }
}
