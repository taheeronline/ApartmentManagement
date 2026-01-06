using ApartmentManagement.Application.Interfaces;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Infrastructure.Repositories
{
    public class ResidentRepository : iResidentRepository
    {
        private readonly ApartmentDbContext _context;

        public ResidentRepository(ApartmentDbContext context)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Resident?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid resident id.", nameof(id));

            try
            {
                return await _context.Residents
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == id);
            }
            catch
            {
                throw;
            }
        }

        public async Task<IReadOnlyList<Resident>> GetAllAsync()
        {
            try
            {
                return await _context.Residents
                    .AsNoTracking()
                    .Include(r => r.Flat)
                    .OrderBy(r => r.MoveInDate)
                    .ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<IReadOnlyList<Resident>> GetByFlatAsync(int flatId)
        {
            if (flatId <= 0)
                throw new ArgumentException("Invalid flat id.", nameof(flatId));

            try
            {
                return await _context.Residents
                    .AsNoTracking()
                    .Where(r => r.FlatId == flatId)
                    .OrderBy(r => r.MoveInDate)
                    .ToListAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> GetActiveResidentCountByFlatAsync(int flatId)
        {
            if (flatId <= 0)
                throw new ArgumentException("Invalid flat id.", nameof(flatId));

            try
            {
                return await _context.Residents
                    .CountAsync(r =>
                        r.FlatId == flatId &&
                        r.MoveOutDate == null);
            }
            catch
            {
                throw;
            }
        }

        public async Task AddAsync(Resident resident)
        {
            if (resident == null)
                throw new ArgumentNullException(nameof(resident));

            try
            {
                _context.Residents.Add(resident);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task UpdateAsync(Resident resident)
        {
            if (resident == null)
                throw new ArgumentNullException(nameof(resident));

            try
            {
                _context.Residents.Update(resident);
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid resident id.", nameof(id));

            try
            {
                var resident = await _context.Residents.FindAsync(id);
                if (resident == null)
                    return;

                _context.Residents.Remove(resident); // replace with soft delete if needed
                await _context.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }
    }
}
