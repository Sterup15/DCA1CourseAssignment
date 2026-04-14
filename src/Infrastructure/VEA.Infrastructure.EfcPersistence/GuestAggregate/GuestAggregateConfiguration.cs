using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VEA.Core.Domain.Aggregates.GuestAggregate;
using VEA.Core.Tools.OperationResult.Result;

namespace VEA.Infrastructure.EfcPersistence.GuestAggregate;

public class GuestAggregateConfiguration : IEntityTypeConfiguration<Guest>
{
    public void Configure(EntityTypeBuilder<Guest> builder)
    {
        builder
            .HasKey(guest => guest.Id);

        builder
            .Property(guest => guest.Id)
            .HasConversion(
                guestId => guestId.Value,
                dbValue => GuestId.From(dbValue).GetValue()
            );

        builder
            .Property(guest => guest.FirstName)
            .HasConversion(
                firstNameVo => firstNameVo.Value,
                name => Name.Create(name).GetValue()
            );

        builder
            .Property(guest => guest.LastName)
            .HasConversion(
                lastNameVo => lastNameVo.Value,
                name => Name.Create(name).GetValue()
            );

        builder
            .Property(guest => guest.ViaMail)
            .HasConversion(
                viaMail => viaMail.Value,
                dbValue => ViaMail.Create(dbValue, false).GetValue()
            );

        builder
            .Property(guest => guest.ProfilePictureUrl)
            .HasConversion(
                uri => uri.ToString(),
                dbValue => new Uri(dbValue)
            );
    }
}