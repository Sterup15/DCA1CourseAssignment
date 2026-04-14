using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VEA.Core.Domain.Aggregates.LocationAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Infrastructure.EfcPersistence.VeaEventAggregate;

public class VeaEventAggregateConfiguration : IEntityTypeConfiguration<VeaEvent>
{
    public void Configure(EntityTypeBuilder<VeaEvent> builder)
    {
        builder.HasKey(e => e.Id);

        builder
            .Property(e => e.Id)
            .HasConversion(
                id => id.Value,
                dbValue => EventId.From(dbValue).GetValue()
            );

        builder
            .Property(e => e.Title)
            .HasConversion(
                title => title.Value,
                dbValue => EventTitle.Create(dbValue).GetValue()
            );

        builder
            .Property(e => e.Description)
            .HasConversion(
                description => description.Value,
                dbValue => EventDescription.Create(dbValue).GetValue()
            );

        builder
            .OwnsOne(e => e.TimeRange, tr =>
            {
                tr.Property(t => t.Start).HasColumnName("TimeRangeStart");
                tr.Property(t => t.End).HasColumnName("TimeRangeEnd");
            });

        builder
            .Property(e => e.Visibility)
            .HasConversion(
                visibility => visibility.Value,
                dbValue => EventVisibility.Create(dbValue).GetValue()
            );

        builder
            .Property(e => e.Status)
            .HasConversion(
                status => status.Value.ToString(),
                dbValue => EventStatus.Create((EventStatusValue)Enum.Parse(typeof(EventStatusValue), dbValue)).GetValue()
            );

        builder
            .Property(e => e.GuestCapacity)
            .HasConversion(
                capacity => capacity.Value,
                dbValue => EventGuestCapacity.Create(dbValue).GetValue()
            );

        builder
            .Property(e => e.Location)
            .HasConversion(
                locationId => locationId!.Value.Value,
                dbValue => LocationId.From(dbValue).GetValue()
            );
    }
}