using ApartmentManagement.Domain.Entities;

namespace ApartmentManagement.Application.Interfaces
{
    public interface iFlatRepository
    {
        Task<Flat?> GetByIdAsync(int id);
        Task<IReadOnlyList<Flat>> GetByApartmentIdAsync(int apartmentId);
        Task AddAsync(Flat flat);
        Task<bool> ExistsAsync(int apartmentId, string flatNumber);
        Task DeleteAsync(int id);
        Task UpdateAsync(Flat flat);


    }
}
