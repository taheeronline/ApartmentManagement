using ApartmentManagement.Application.Interfaces;
using Microsoft.Extensions.Logging;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ApartmentManagement.Infrastructure.Repositories
{
    public class ResidentRepository : iResidentRepository
    {
        private readonly ApartmentDbContext _context;
        private readonly ILogger<ResidentRepository> _logger;

        public ResidentRepository(ApartmentDbContext context, ILogger<ResidentRepository> logger)
        {
            _context = context
                ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Resident?> GetByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid resident id.", nameof(id));

            return await _context.Residents
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IReadOnlyList<Resident>> GetAllAsync()
        {
            return await _context.Residents
                .AsNoTracking()
                .Include(r => r.Flat)
                .OrderBy(r => r.MoveInDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Resident>> GetByFlatAsync(int flatId)
        {
            if (flatId <= 0)
                throw new ArgumentException("Invalid flat id.", nameof(flatId));

            return await _context.Residents
                .AsNoTracking()
                .Where(r => r.FlatId == flatId)
                .OrderBy(r => r.MoveInDate)
                .ToListAsync();
        }

        public async Task<int> GetActiveResidentCountByFlatAsync(int flatId)
        {
            if (flatId <= 0)
                throw new ArgumentException("Invalid flat id.", nameof(flatId));

            return await _context.Residents
                .CountAsync(r =>
                    r.FlatId == flatId &&
                    r.MoveOutDate == null);
        }

        public async Task AddAsync(Resident resident)
        {
            if (resident == null)
                throw new ArgumentNullException(nameof(resident));

            _context.Residents.Add(resident);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Added resident {FullName} to flat {FlatId}", resident.FullName, resident.FlatId);
        }

        public async Task UpdateAsync(Resident resident)
        {
            if (resident == null)
                throw new ArgumentNullException(nameof(resident));

            _context.Residents.Update(resident);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated resident {ResidentId}", resident.Id);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid resident id.", nameof(id));

            var resident = await _context.Residents.FindAsync(id);
            if (resident == null)
                return;

            _context.Residents.Remove(resident); // replace with soft delete if needed
            await _context.SaveChangesAsync();
            _logger.LogInformation("Deleted resident {ResidentId}", id);
        }
    }
}
