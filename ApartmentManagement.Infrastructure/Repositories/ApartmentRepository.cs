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
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task AddAsync(Apartment apartment)
        {
            if (apartment == null)
                throw new ArgumentNullException(nameof(apartment));

            try
            {
                await _context.Apartments.AddAsync(apartment);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<IReadOnlyList<Apartment>> GetAllAsync()
        {
            try
            {
                return await _context.Apartments
                    .AsNoTracking()
                    .Include(a => a.Flats)
                    .ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<Apartment?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid apartment id.", nameof(id));

            try
            {
                return await _context.Apartments
                    .Include(a => a.Flats)
                    .FirstOrDefaultAsync(a => a.Id == id);
            }
            catch
            {
                throw;
            }
        }
    }
}
