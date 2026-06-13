using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using VEA.Infrastructure.EfcQueries.Models;

namespace VEA.Infrastructure.EfcQueries;

public partial class VeadatabaseProductionContext : DbContext
{
    public VeadatabaseProductionContext()
    {
    }

    public VeadatabaseProductionContext(DbContextOptions<VeadatabaseProductionContext> options)
        : base(options)
    {
    }

    public virtual DbSet<EfmigrationsLock> EfmigrationsLocks { get; set; }

    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<Participation> Participations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
            optionsBuilder.UseSqlite("Data Source = D:\\Repos\\DCA1\\VEA\\src\\Infrastructure\\VEA.Infrastructure.EfcPersistence\\VEADatabaseProduction.db");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EfmigrationsLock>(entity =>
        {
            entity.ToTable("__EFMigrationsLock");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Participation>(entity =>
        {
            entity.ToTable("Participation");

            entity.HasIndex(e => e.EventId, "IX_Participation_EventId");

            entity.HasIndex(e => e.GuestId, "IX_Participation_GuestId");

            entity.HasOne(d => d.Event).WithMany(p => p.Participations).HasForeignKey(d => d.EventId);

            entity.HasOne(d => d.Guest).WithMany(p => p.Participations).HasForeignKey(d => d.GuestId);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
