using ApartmentManagement.Domain.Entities;

namespace ApartmentManagement.Application.Interfaces
{
    public interface iApartmentRepository
    {
        Task<Apartment?> GetByIdAsync(int id);
        Task<IReadOnlyList<Apartment>> GetAllAsync();
        Task AddAsync(Apartment apartment);
        Task UpdateAsync(Apartment apartment);
        Task DeleteAsync(Apartment apartment);
    }
}
