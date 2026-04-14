using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate;
using VEA.Core.Domain.Aggregates.VeaEventAggregate.ParticipationEntity;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Infrastructure.EfcPersistence.VeaEventAggregate;

public class ParticipationConfiguration : IEntityTypeConfiguration<Participation>
{
    public void Configure(EntityTypeBuilder<Participation> builder)
    {
        builder.HasKey(p => p.Id);

        builder
            .Property(p => p.Id)
            .HasConversion(
                id => id.Value,
                dbValue => ParticipationId.From(dbValue).GetValue()
            );

        builder
            .Property(p => p.GuestId)
            .HasConversion(
                guestId => guestId.Value,
                dbValue => GuestId.From(dbValue).GetValue()
            );

        builder
            .Property(p => p.EventId)
            .HasConversion(
                eventId => eventId.Value,
                dbValue => EventId.From(dbValue).GetValue()
            );

        builder
            .Property(p => p.Status)
            .HasConversion(
                status => status.Value.ToString(),
                dbValue => ParticipationStatus.Create((ParticipationStatusValue)Enum.Parse(typeof(ParticipationStatusValue), dbValue)).GetValue()
            );

        builder
            .Property(p => p.Source)
            .HasConversion(
                source => source.Value,
                dbValue => ParticipationSource.Create(dbValue).GetValue()
            );

        builder
            .Property(p => p.JoinReason)
            .HasConversion(
                joinReason => joinReason.Value,
                dbValue => ParticipationJoinReason.Create(dbValue).GetValue()
            );

        builder
            .HasOne<Guest>()
            .WithMany()
            .HasForeignKey(p => p.GuestId);

        builder
            .HasOne<VeaEvent>()
            .WithMany("_participations")
            .HasForeignKey(p => p.EventId);
    }
}