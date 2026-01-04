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
    }
}
