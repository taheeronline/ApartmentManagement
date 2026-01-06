using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagement.Infrastructure.Persistence.Configuration
{
    public class ResidentConfiguration : IEntityTypeConfiguration<Resident>
    {
        public void Configure(EntityTypeBuilder<Resident> builder)
        {
            builder.ToTable("Residents");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.FullName)
                   .IsRequired()
                   .HasMaxLength(150);

            builder.Property(r => r.PhoneNumber)
                   .HasMaxLength(20);

            builder.Property(r => r.Email)
                   .HasMaxLength(70);

            builder.Property(r => r.ResidentType)
                   .IsRequired()
                   .HasConversion<int>();

            builder.Property(r => r.MoveInDate)
                   .IsRequired();

            builder.Property(r => r.MoveOutDate);

            builder.HasOne(r => r.Flat)
                   .WithMany(f => f.Residents)
                   .HasForeignKey(r => r.FlatId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }

}
