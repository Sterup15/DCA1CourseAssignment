using IntegrationTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.UpdateEventTimeRange;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace IntegrationTests.Features.EventAggregate.UpdateEventTimeRange;

public class UpdateEventTimeRangeHandlerIntegrationTests : IntegrationTestBase
{
    [Fact]
    public async Task HandleAsync_WhenEventExists_UpdatesTimeRangeAndPersists()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        await DbContext.Events.AddAsync(veaEvent);
        await DbContext.SaveChangesAsync();

        var start = new DateTime(2030, 8, 25, 13, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2030, 8, 25, 15, 0, 0, DateTimeKind.Utc);
        var command = Assert.IsType<Success<UpdateEventTimeRangeCommand>>(
            UpdateEventTimeRangeCommand.Create(veaEvent.Id.ToString(), start, end)).Value;
        var handler = new UpdateEventTimeRangeHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
        await using var fresh = CreateFreshContext();
        var savedEvent = await fresh.Events.FindAsync(veaEvent.Id);
        Assert.Equal(start, savedEvent!.TimeRange!.Start);
        Assert.Equal(end, savedEvent.TimeRange.End);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var start = new DateTime(2030, 8, 25, 13, 0, 0, DateTimeKind.Utc);
        var end = new DateTime(2030, 8, 25, 15, 0, 0, DateTimeKind.Utc);
        var command = Assert.IsType<Success<UpdateEventTimeRangeCommand>>(
            UpdateEventTimeRangeCommand.Create(EventId.New().ToString(), start, end)).Value;
        var handler = new UpdateEventTimeRangeHandler(EventRepository, UnitOfWork);

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
