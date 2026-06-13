using VEA.Infrastructure.EfcQueries.SeedFactories;

namespace VEA.Infrastructure.EfcQueries;

public static class DatabaseContextExtensions
{
    public static VeadatabaseProductionContext Seed(this VeadatabaseProductionContext context)
    {
        context.Guests.AddRange(GuestSeedFactory.CreateGuests());
        context.Events.AddRange(EventSeedFactory.CreateEvents());
        context.SaveChanges();

        ParticipationSeedFactory.Seed(context);
        context.SaveChanges();

        return context;
    }
}
