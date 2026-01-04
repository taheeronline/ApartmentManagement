using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ApartmentManagement.Infrastructure.Persistence
{
    public class ApartmentDbContextFactory
    : IDesignTimeDbContextFactory<ApartmentDbContext>
    {
        public ApartmentDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApartmentDbContext>();

            optionsBuilder.UseSqlServer(
                "Server=.;Database=ApartmentManagementDb;Trusted_Connection=True;TrustServerCertificate=True");

            return new ApartmentDbContext(optionsBuilder.Options);
        }
    }
}
