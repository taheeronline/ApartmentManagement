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
            _context = context;
        }

        public async Task AddAsync(Flat flat)
        {
            await _context.Flats.AddAsync(flat);
            await _context.SaveChangesAsync();
        }

        public async Task<Flat?> GetByIdAsync(int id)
        {
            return await _context.Flats
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.Id == id);
        }

        public async Task<IReadOnlyList<Flat>> GetByApartmentIdAsync(int apartmentId)
        {
            return await _context.Flats
                .AsNoTracking()
                .Where(f => f.ApartmentId == apartmentId)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int apartmentId, string flatNumber)
        {
            return await _context.Flats
                .AnyAsync(f =>
                    f.ApartmentId == apartmentId &&
                    f.FlatNumber == flatNumber);
        }

        public async Task DeleteAsync(int id)
        {
            var flat = await _context.Flats.FindAsync(id);
            if (flat == null) return;

            _context.Flats.Remove(flat);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Flat flat)
        {
            _context.Flats.Update(flat);
            await _context.SaveChangesAsync();
        }


    }
}
