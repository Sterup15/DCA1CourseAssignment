using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VEA.Infrastructure.EfcPersistence.Common;

public class DesignTimeContextFactory : IDesignTimeDbContextFactory<EfcWriteDbContext>
{
    public EfcWriteDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<EfcWriteDbContext>();
        optionsBuilder.UseSqlite(@"Data Source = VEADatabaseProduction.db");
        return new EfcWriteDbContext(optionsBuilder.Options);
    }
}