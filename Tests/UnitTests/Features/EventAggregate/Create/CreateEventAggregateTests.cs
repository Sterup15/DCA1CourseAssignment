using VEA.Core.Domain.Aggregates.VeaEventAggregate;

namespace UnitTests.Features.EventAggregate.Create;

public class CreateEventAggregateTests
{
    [Fact]
    public void Can_Create_Valid_Default_Event()
    {
        // Arrange
        EventId id = EventId.New();
        
        // Act
        VeaEvent evt = VeaEvent.Create(id);
        // Assert
        Assert.Equal(id, evt.Id);
        Assert.Equal(EventStatus.Draft, evt.Status);
        Assert.Equal(5, evt.MaxNoOfGuests);
        Assert.Equal("Working Title", evt.Title);
        Assert.Equal("", evt.Description);
        Assert.Equal(EventVisibility.Private, evt.Visibility);
        
    }
}