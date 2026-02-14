using ApartmentManagement.Application.Interfaces;
using Microsoft.Extensions.Logging;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Infrastructure.Repositories
{
    public class ApartmentRepository : iApartmentRepository
    {
        private readonly ApartmentDbContext _context;
        private readonly ILogger<ApartmentRepository> _logger;

        public ApartmentRepository(ApartmentDbContext context, ILogger<ApartmentRepository> logger)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddAsync(Apartment apartment)
        {
            if (apartment == null)
                throw new ArgumentNullException(nameof(apartment));

            await _context.Apartments.AddAsync(apartment);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added apartment {ApartmentName}", apartment.Name);
        }

        public async Task<IReadOnlyList<Apartment>> GetAllAsync()
        {
            return await _context.Apartments
                .AsNoTracking()
                .Include(a => a.Flats)
                .ToListAsync();
        }

        public async Task<Apartment?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid apartment id.", nameof(id));

            return await _context.Apartments
                .Include(a => a.Flats)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task UpdateAsync(Apartment apartment)
        {
            if (apartment == null)
                throw new ArgumentNullException(nameof(apartment));

            _context.Apartments.Update(apartment);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated apartment {ApartmentName}", apartment.Name);
        }

        public async Task DeleteAsync(Apartment apartment)
        {
            if (apartment == null)
                throw new ArgumentNullException(nameof(apartment));

            _context.Apartments.Remove(apartment);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted apartment {ApartmentName}", apartment.Name);
        }
    }
}
