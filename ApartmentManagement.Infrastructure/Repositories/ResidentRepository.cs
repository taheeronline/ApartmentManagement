using ApartmentManagement.Application.Interfaces;
using ApartmentManagement.Domain.Entities;
using ApartmentManagement.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagement.Infrastructure.Repositories
{
    public class ResidentRepository : iResidentRepository
    {
        private readonly ApartmentDbContext _context;

        public ResidentRepository(ApartmentDbContext context)
        {
            _context = context;
        }

        public async Task<Resident?> GetByIdAsync(int id)
        {
            return await _context.Residents
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IReadOnlyList<Resident>> GetByFlatAsync(int flatId)
        {
            return await _context.Residents
                .Where(r => r.FlatId == flatId)
                .OrderBy(r => r.MoveInDate)
                .ToListAsync();
        }

        public async Task<int> GetActiveResidentCountByFlatAsync(int flatId)
        {
            return await _context.Residents
                .CountAsync(r => r.FlatId == flatId && r.MoveOutDate == null);
        }

        public async Task AddAsync(Resident resident)
        {
            _context.Residents.Add(resident);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Resident resident)
        {
            _context.Residents.Update(resident);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var resident = await _context.Residents.FindAsync(id);
            if (resident == null) return;

            _context.Residents.Remove(resident); // OR soft delete if enabled
            await _context.SaveChangesAsync();
        }
    }

}
