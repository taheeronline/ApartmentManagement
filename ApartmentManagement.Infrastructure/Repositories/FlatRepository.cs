using ApartmentManagement.Application.Interfaces;
using Microsoft.Extensions.Logging;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Infrastructure.Repositories
{
    public class FlatRepository : iFlatRepository
    {
        private readonly ApartmentDbContext _context;
        private readonly ILogger<FlatRepository> _logger;

        public FlatRepository(ApartmentDbContext context, ILogger<FlatRepository> logger)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddAsync(Flat flat)
        {
            if (flat == null)
                throw new ArgumentNullException(nameof(flat));

            await _context.Flats.AddAsync(flat);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added flat {FlatNumber} (Id: {FlatId}) to apartment {ApartmentId}", flat.FlatNumber, flat.Id, flat.ApartmentId);
        }

        public async Task<Flat?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid flat id.", nameof(id));

            return await _context.Flats
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IReadOnlyList<Flat>> GetByApartmentIdAsync(int apartmentId)
        {
            if (apartmentId <= 0)
                throw new ArgumentException("Invalid apartment id.", nameof(apartmentId));

            return await _context.Flats
                .AsNoTracking()
                .Where(f => f.ApartmentId == apartmentId)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Flat>> GetAllAsync()
        {
            return await _context.Flats
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int apartmentId, string flatNumber)
        {
            if (apartmentId <= 0)
                throw new ArgumentException("Invalid apartment id.", nameof(apartmentId));

            if (string.IsNullOrWhiteSpace(flatNumber))
                throw new ArgumentException("Flat number is required.", nameof(flatNumber));

            return await _context.Flats.AnyAsync(f =>
                f.ApartmentId == apartmentId &&
                f.FlatNumber == flatNumber);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid flat id.", nameof(id));

            var flat = await _context.Flats.FindAsync(id);
            if (flat == null)
                return;

            _context.Flats.Remove(flat);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted flat {FlatId}", id);
        }

        public async Task UpdateAsync(Flat flat)
        {
            if (flat == null)
                throw new ArgumentNullException(nameof(flat));

            _context.Flats.Update(flat);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated flat {FlatId}", flat.Id);
        }
    }
}
