using IntegrationTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.ReadyEvent;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace IntegrationTests.Features.EventAggregate.ReadyEvent;

public class ReadyEventHandlerIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task HandleAsync_WhenEventHasRequiredFields_ReadiesEventAndPersists()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var now = VeaEventTestFactory.DefaultNow;
        veaEvent.UpdateTitle(VeaEventTestFactory.CreateTitle("Valid Title"));
        veaEvent.UpdateDescription(VeaEventTestFactory.CreateDescription("Valid description"));
        veaEvent.UpdateTimeRange(
            VeaEventTestFactory.CreateTimeRange(
                new DateTime(2030, 08, 25, 13, 00, 00, DateTimeKind.Utc),
                new DateTime(2030, 08, 25, 15, 00, 00, DateTimeKind.Utc)),
            now);
        await DbContext.Events.AddAsync(veaEvent);
        await DbContext.SaveChangesAsync();

        var command = Assert.IsType<Success<ReadyEventCommand>>(
            ReadyEventCommand.Create(veaEvent.Id.ToString())).Value;
        var handler = new ReadyEventHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
        await using var fresh = CreateFreshContext();
        var savedEvent = await fresh.Events.FindAsync(veaEvent.Id);
        Assert.Equal(EventStatusValue.Ready, savedEvent!.Status.Value);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<ReadyEventCommand>>(
            ReadyEventCommand.Create(EventId.New().ToString())).Value;
        var handler = new ReadyEventHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
