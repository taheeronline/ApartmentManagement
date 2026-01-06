using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Infrastructure.Persistence.Configuration
{
    public class FlatConfiguration : IEntityTypeConfiguration<Flat>
    {
        public void Configure(EntityTypeBuilder<Flat> builder)
        {
            builder.ToTable("Flats");

            builder.HasKey(f => f.Id);

            builder.Property(f => f.FlatNumber)
                   .IsRequired()
                   .HasMaxLength(50);

            builder.Property(f => f.Floor)
                   .IsRequired();

            builder.Navigation(f => f.Residents)
                   .UsePropertyAccessMode(PropertyAccessMode.Field);

        }
    }
}
