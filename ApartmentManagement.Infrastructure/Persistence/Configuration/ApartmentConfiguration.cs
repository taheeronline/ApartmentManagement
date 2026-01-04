using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ApartmentManagement.Infrastructure.Persistence.Configuration
{
    public class ApartmentConfiguration : IEntityTypeConfiguration<Apartment>
    {
        public void Configure(EntityTypeBuilder<Apartment> builder)
        {
            builder.ToTable("Apartments");

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Name)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(a => a.Address)
                   .IsRequired()
                   .HasMaxLength(500);

            builder.HasMany(a => a.Flats)
                   .WithOne()
                   .HasForeignKey(f => f.ApartmentId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
