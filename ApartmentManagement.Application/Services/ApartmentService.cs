using ApartmentManagement.Application.Interfaces;
using ApartmentManagement.Domain.Entities;

namespace ApartmentManagement.Application.Services
{
    public class ApartmentService
    {
        private readonly iApartmentRepository _apartmentRepository;

        public ApartmentService(iApartmentRepository apartmentRepository)
        {
            _apartmentRepository = apartmentRepository;
        }

        public Task<IReadOnlyList<Apartment>> GetApartmentsAsync()
        {
            return _apartmentRepository.GetAllAsync();
        }

        public Task AddApartmentAsync(string name, string address)
        {
            var apartment = new Apartment(name, address);
            return _apartmentRepository.AddAsync(apartment);
        }
    }
}
