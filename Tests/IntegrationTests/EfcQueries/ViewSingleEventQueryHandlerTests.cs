using IntegrationTests.EfcQueries.Common;
using VEA.Infrastructure.EfcQueries;
using VEA.Core.QueryContracts.Queries;
using VEA.Infrastructure.EfcQueries.Queries;

namespace IntegrationTests.EfcQueries;

// Seed events referenced below:
//   evt-01  Friday Bar             Active, Public,  cap=50, attendees: g1(Doe), g2(Smith), g3(Johnson)
//   evt-03  Board Game Night       Active, Public,  cap=20, attendees: g7(Miller), g8(Taylor)
//   evt-04  Private Networking     Active, Private, cap=15, g1 Attending, g3 Invited, g5 Rejected → 1 attendee
public class ViewSingleEventQueryHandlerTests
{
    private const string Evt01 = "20000000-0000-0000-0000-000000000001";
    private const string Evt03 = "20000000-0000-0000-0000-000000000003";
    private const string Evt04 = "20000000-0000-0000-0000-000000000004";

    [Fact]
    public async Task ReturnsCorrectEventDetails()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new ViewSingleEventQueryHandler(ctx);

        var answer = await handler.HandleAsync(new ViewSingleEventQuery(Evt01, 0, 9));

        Assert.Equal(Evt01, answer.Id);
        Assert.Equal("Friday Bar", answer.Title);
        Assert.Equal("Come for cheap beer and great company.", answer.Description);
        Assert.Equal("2024-03-01T18:00:00", answer.Start);
        Assert.Equal("2024-03-01T23:00:00", answer.End);
        Assert.True(answer.IsPublic);
        Assert.Equal(50, answer.GuestCapacity);
    }

    [Fact]
    public async Task AttendeeCountReflectsOnlyAttendingParticipations()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new ViewSingleEventQueryHandler(ctx);

        // evt-04 has 3 participations but only 1 is Attending
        var answer = await handler.HandleAsync(new ViewSingleEventQuery(Evt04, 0, 9));

        Assert.Equal(1, answer.AttendeeCount);
    }

    [Fact]
    public async Task PrivateEventHasIsPublicFalse()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new ViewSingleEventQueryHandler(ctx);

        var answer = await handler.HandleAsync(new ViewSingleEventQuery(Evt04, 0, 9));

        Assert.False(answer.IsPublic);
    }

    [Fact]
    public async Task AttendeeListExcludesInvitedAndRejected()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new ViewSingleEventQueryHandler(ctx);

        // evt-04: g1 Attending, g3 Invited, g5 Rejected → only g1 in list
        var answer = await handler.HandleAsync(new ViewSingleEventQuery(Evt04, 0, 9));

        Assert.Single(answer.Attendees);
        Assert.Equal("John Doe", answer.Attendees.Single().FullName);
    }

    [Fact]
    public async Task AttendeesOrderedByLastName()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new ViewSingleEventQueryHandler(ctx);

        // evt-01 attendees: Doe, Johnson, Smith (alphabetical)
        var answer = await handler.HandleAsync(new ViewSingleEventQuery(Evt01, 0, 9));
        var names = answer.Attendees.Select(a => a.FullName).ToList();

        Assert.Equal("John Doe", names[0]);
        Assert.Equal("Alice Johnson", names[1]);
        Assert.Equal("Jane Smith", names[2]);
    }

    [Fact]
    public async Task GuestOffsetSkipsAttendees()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new ViewSingleEventQueryHandler(ctx);

        // offset=1 skips "John Doe", so first result is "Alice Johnson"
        var answer = await handler.HandleAsync(new ViewSingleEventQuery(Evt01, 1, 9));

        Assert.Equal(2, answer.Attendees.Count());
        Assert.Equal("Alice Johnson", answer.Attendees.First().FullName);
    }

    [Fact]
    public async Task GuestPageSizeLimitsAttendeeList()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new ViewSingleEventQueryHandler(ctx);

        var answer = await handler.HandleAsync(new ViewSingleEventQuery(Evt01, 0, 1));

        Assert.Single(answer.Attendees);
        Assert.Equal(3, answer.AttendeeCount); // total unchanged
    }

    [Fact]
    public async Task AttendeeHasProfilePictureUrl()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new ViewSingleEventQueryHandler(ctx);

        var answer = await handler.HandleAsync(new ViewSingleEventQuery(Evt03, 0, 9));

        Assert.All(answer.Attendees, a => Assert.False(string.IsNullOrEmpty(a.ProfilePictureUrl)));
    }
}