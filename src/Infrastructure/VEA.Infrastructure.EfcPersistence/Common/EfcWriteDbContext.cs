using Microsoft.EntityFrameworkCore;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;

namespace VEA.Infrastructure.EfcPersistence.Common;

public class EfcWriteDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<VeaEvent> Events => Set<VeaEvent>();
    public DbSet<Guest> Guests => Set<Guest>();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EfcWriteDbContext).Assembly);
    }
    
}