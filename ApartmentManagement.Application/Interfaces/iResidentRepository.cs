using ApartmentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagement.Application.Interfaces
{
    public interface iResidentRepository
    {
        Task<Resident?> GetByIdAsync(int id);

        Task<IReadOnlyList<Resident>> GetByFlatAsync(int flatId);

        Task<int> GetActiveResidentCountByFlatAsync(int flatId);

        Task AddAsync(Resident resident);

        Task UpdateAsync(Resident resident);

        Task DeleteAsync(int id);
    }

}
