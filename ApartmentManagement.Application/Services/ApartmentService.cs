using ApartmentManagement.Application.DTOs;
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

        public async Task<IReadOnlyList<ApartmentDto>> GetApartmentsAsync()
        {
            var apartments = await _apartmentRepository.GetAllAsync();

            return apartments.Select(a => new ApartmentDto
            {
                Id = a.Id,
                Name = a.Name,
                Address = a.Address,
                FlatCount = a.Flats.Count
            }).ToList();
        }


        public Task AddApartmentAsync(string name, string address)
        {
            var apartment = new Apartment(name, address);
            return _apartmentRepository.AddAsync(apartment);
        }
    }
}
