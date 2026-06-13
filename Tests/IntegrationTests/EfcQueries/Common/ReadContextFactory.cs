using Microsoft.EntityFrameworkCore;
using VEA.Infrastructure.EfcQueries;

namespace IntegrationTests.EfcQueries.Common;

public static class ReadContextFactory
{
    public static VeadatabaseProductionContext Create()
    {
        var optionsBuilder = new DbContextOptionsBuilder<VeadatabaseProductionContext>();
        optionsBuilder.UseSqlite("Data Source = Test" + Guid.NewGuid() + ".db");
        var ctx = new VeadatabaseProductionContext(optionsBuilder.Options);
        ctx.Database.EnsureDeleted();
        ctx.Database.EnsureCreated();
        return ctx;
    }
}