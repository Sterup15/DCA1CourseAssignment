using IntegrationTests.EfcQueries.Common;
using VEA.Infrastructure.EfcQueries;

namespace IntegrationTests.EfcQueries;

public class DbSeedTest
{
    [Fact]
    public void DbHasGuestsSeed()
    {
        using var ctx = ReadContextFactory.Create().Seed();
        Assert.NotEmpty(ctx.Guests);
        Assert.Equal(10, ctx.Guests.Count());
    }

    [Fact]
    public void DbHasEventsSeed()
    {
        using var ctx = ReadContextFactory.Create().Seed();
        Assert.NotEmpty(ctx.Events);
        Assert.Equal(8, ctx.Events.Count());
    }

    [Fact]
    public void DbHasParticipationsSeed()
    {
        using var ctx = ReadContextFactory.Create().Seed();
        Assert.NotEmpty(ctx.Participations);
        Assert.Equal(13, ctx.Participations.Count());
    }
}