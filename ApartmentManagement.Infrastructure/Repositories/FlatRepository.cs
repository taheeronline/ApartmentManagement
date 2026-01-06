using ApartmentManagement.Application.Interfaces;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Infrastructure.Repositories
{
    public class FlatRepository : iFlatRepository
    {
        private readonly ApartmentDbContext _context;

        public FlatRepository(ApartmentDbContext context)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(Flat flat)
        {
            if (flat == null)
                throw new ArgumentNullException(nameof(flat));

            try
            {
                await _context.Flats.AddAsync(flat);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<Flat?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid flat id.", nameof(id));

            try
            {
                return await _context.Flats
                    .AsNoTracking()
                    .FirstOrDefaultAsync(f => f.Id == id);
            }
            catch
            {
                throw;
            }
        }

        public async Task<IReadOnlyList<Flat>> GetByApartmentIdAsync(int apartmentId)
        {
            if (apartmentId <= 0)
                throw new ArgumentException("Invalid apartment id.", nameof(apartmentId));

            try
            {
                return await _context.Flats
                    .AsNoTracking()
                    .Where(f => f.ApartmentId == apartmentId)
                    .ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<IReadOnlyList<Flat>> GetAllAsync()
        {
            try
            {
                return await _context.Flats
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> ExistsAsync(int apartmentId, string flatNumber)
        {
            if (apartmentId <= 0)
                throw new ArgumentException("Invalid apartment id.", nameof(apartmentId));

            if (string.IsNullOrWhiteSpace(flatNumber))
                throw new ArgumentException("Flat number is required.", nameof(flatNumber));

            try
            {
                return await _context.Flats.AnyAsync(f =>
                    f.ApartmentId == apartmentId &&
                    f.FlatNumber == flatNumber);
            }
            catch
            {
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid flat id.", nameof(id));

            try
            {
                var flat = await _context.Flats.FindAsync(id);
                if (flat == null)
                    return;

                _context.Flats.Remove(flat);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateAsync(Flat flat)
        {
            if (flat == null)
                throw new ArgumentNullException(nameof(flat));

            try
            {
                _context.Flats.Update(flat);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
