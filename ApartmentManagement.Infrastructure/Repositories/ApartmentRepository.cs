using ApartmentManagement.Application.Interfaces;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Infrastructure.Repositories
{
    public class ApartmentRepository : iApartmentRepository
    {
        private readonly ApartmentDbContext _context;

        public ApartmentRepository(ApartmentDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Apartment apartment)
        {
            await _context.Apartments.AddAsync(apartment);
            await _context.SaveChangesAsync();
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
            return await _context.Apartments
                .Include(a => a.Flats)
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
