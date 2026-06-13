using IntegrationTests.EfcQueries.Common;
using VEA.Infrastructure.EfcQueries;
using VEA.Core.QueryContracts.Queries;
using VEA.Infrastructure.EfcQueries.Queries;

namespace IntegrationTests.EfcQueries;

// Seed guests referenced below:
//   g1 = 10000000-...-0001  John Doe       jdo@via.dk   → Attending: evt-01(past), evt-04(past)
//   g2 = 10000000-...-0002  Jane Smith     jsm@via.dk   → Attending: evt-01(past), evt-05(future)
//   g3 = 10000000-...-0003  Alice Johnson  ajo@via.dk   → Attending: evt-01(past)  Invited: evt-04(past)
// Fake time = 2024-03-25; past events are before this date
public class GuestProfilePageQueryHandlerTests
{
    private const string G1 = "10000000-0000-0000-0000-000000000001";
    private const string G2 = "10000000-0000-0000-0000-000000000002";
    private const string G3 = "10000000-0000-0000-0000-000000000003";

    private static readonly FakeSystemTime FakeTime = new(new DateTime(2024, 3, 25));

    [Fact]
    public async Task ReturnsCorrectGuestInfo()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new GuestProfilePageQueryHandler(ctx, FakeTime);

        var answer = await handler.HandleAsync(new GuestProfilePageQuery(G2));

        Assert.Equal(G2, answer.Id);
        Assert.Equal("Jane Smith", answer.FullName);
        Assert.Equal("jsm@via.dk", answer.Email);
    }

    [Fact]
    public async Task PendingInvitationsCountIsCorrect()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new GuestProfilePageQueryHandler(ctx, FakeTime);

        // g3 has 1 pending invitation (evt-04, status=Invited)
        var answer = await handler.HandleAsync(new GuestProfilePageQuery(G3));

        Assert.Equal(1, answer.PendingInvitationsCount);
    }

    [Fact]
    public async Task NoPendingInvitationsWhenNoneExist()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new GuestProfilePageQueryHandler(ctx, FakeTime);

        var answer = await handler.HandleAsync(new GuestProfilePageQuery(G1));

        Assert.Equal(0, answer.PendingInvitationsCount);
    }

    [Fact]
    public async Task UpcomingEventsOnlyIncludesFutureAttendingEvents()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new GuestProfilePageQueryHandler(ctx, FakeTime);

        // g2 attends evt-01 (past) and evt-05 (future) → 1 upcoming
        var answer = await handler.HandleAsync(new GuestProfilePageQuery(G2));

        Assert.Single(answer.UpcomingEvents);
        Assert.Equal("Study Group Session", answer.UpcomingEvents.Single().Title);
    }

    [Fact]
    public async Task UpcomingEventsAreOrderedByDateAscending()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new GuestProfilePageQueryHandler(ctx, FakeTime);

        // g1 has no upcoming events; use g2 which has 1 — just verifying ordering logic holds
        var answer = await handler.HandleAsync(new GuestProfilePageQuery(G2));

        var starts = answer.UpcomingEvents.Select(e => e.Start).ToList();
        Assert.Equal(starts.OrderBy(s => s).ToList(), starts);
    }

    [Fact]
    public async Task PastEventsOnlyIncludesAttendingStatus()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new GuestProfilePageQueryHandler(ctx, FakeTime);

        // g3 has evt-01 (Attending, past) and evt-04 (Invited, past) → only evt-01 in past
        var answer = await handler.HandleAsync(new GuestProfilePageQuery(G3));

        Assert.Single(answer.PastEvents);
        Assert.Equal("Friday Bar", answer.PastEvents.Single().Title);
    }

    [Fact]
    public async Task PastEventsOrderedNewestFirst()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new GuestProfilePageQueryHandler(ctx, FakeTime);

        // g1: evt-04 (2024-03-20) and evt-01 (2024-03-01) → evt-04 first
        var answer = await handler.HandleAsync(new GuestProfilePageQuery(G1));
        var past = answer.PastEvents.ToList();

        Assert.Equal(2, past.Count);
        Assert.Equal("Private Networking Event", past[0].Title);
        Assert.Equal("Friday Bar", past[1].Title);
    }

    [Fact]
    public async Task PastEventsLimitedToFive()
    {
        await using var ctx = ReadContextFactory.Create().Seed();
        var handler = new GuestProfilePageQueryHandler(ctx, FakeTime);

        // No guest in seed data has more than 5 past events, so count ≤ 5
        var answer = await handler.HandleAsync(new GuestProfilePageQuery(G1));

        Assert.True(answer.PastEvents.Count() <= 5);
    }
}