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
            _apartmentRepository = apartmentRepository
                ?? throw new ArgumentNullException(nameof(apartmentRepository));
        }

        public async Task<IReadOnlyList<ApartmentDto>> GetApartmentsAsync()
        {
            try
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
            catch
            {
                throw;
            }
        }

        public async Task AddApartmentAsync(string name, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Apartment name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Apartment address is required.", nameof(address));

            try
            {
                var apartment = new Apartment(name, address);
                await _apartmentRepository.AddAsync(apartment);
            }
            catch
            {
                throw;
            }
        }
    }
}
