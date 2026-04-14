using UnitTests.Common;
using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Application.Features.VeaEventHandlers.UpdateEventDescription;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.UpdateEventDescription;

public class UpdateEventDescriptionHandlerTests
{
    [Fact]
    public async Task HandleAsync_WhenEventExists_ReturnsSuccess()
    {
        // Arrange
        var veaEvent = VeaEventTestFactory.CreateEvent();
        var db = new FakeDbContext();
        db.Events.Add(veaEvent);
        await db.SaveChangesAsync();
        var command = Assert.IsType<Success<UpdateEventDescriptionCommand>>(
            UpdateEventDescriptionCommand.Create(veaEvent.Id.ToString(), "A new description.")).Value;
        var handler = new UpdateEventDescriptionHandler(new FakeVeaEventRepository(db), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        Assert.IsType<Success<None>>(result);
    }

    [Fact]
    public async Task HandleAsync_WhenEventNotFound_ReturnsFailure()
    {
        // Arrange
        var command = Assert.IsType<Success<UpdateEventDescriptionCommand>>(
            UpdateEventDescriptionCommand.Create(EventId.New().ToString(), "A new description.")).Value;
        var handler = new UpdateEventDescriptionHandler(new FakeVeaEventRepository(new FakeDbContext()), new FakeUnitOfWork());

        // Act
        var result = await handler.HandleAsync(command);

        // Assert
        var failure = Assert.IsType<Failure<None>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.VeaEvent.EventNotFound);
    }
}
