using VEA.Core.Application.AppEntry.Commands.VeaEventCommands;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult;
using VEA.Core.Tools.OperationResult.Result;

namespace UnitTests.Features.EventAggregate.UpdateEventDescription;

public class UpdateEventDescriptionCommandTests
{
    [Fact]
    public void Create_WithValidInputs_ReturnsSuccess()
    {
        // Arrange
        var eventId = EventId.New().ToString();

        // Act
        var result = UpdateEventDescriptionCommand.Create(eventId, "A valid description.");

        // Assert
        Assert.IsType<Success<UpdateEventDescriptionCommand>>(result);
    }

    [Fact]
    public void Create_WithDescriptionTooLong_ReturnsFailure()
    {
        // Arrange
        var eventId = EventId.New().ToString();
        var tooLong = new string('A', 251);

        // Act
        var result = UpdateEventDescriptionCommand.Create(eventId, tooLong);

        // Assert
        var failure = Assert.IsType<Failure<UpdateEventDescriptionCommand>>(result);
        Assert.Contains(failure.Errors, e => e == EventErrors.EventDescription.TooLong);
    }
}
