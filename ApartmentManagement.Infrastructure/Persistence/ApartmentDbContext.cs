using ApartmentManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Infrastructure.Persistence
{
    public class ApartmentDbContext : DbContext
    {
        public ApartmentDbContext(DbContextOptions<ApartmentDbContext> options)
            : base(options)
        {
        }

        public DbSet<Apartment> Apartments => Set<Apartment>();
        public DbSet<Flat> Flats => Set<Flat>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApartmentDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
