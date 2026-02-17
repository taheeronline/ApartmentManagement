using System.Linq;
using ApartmentManagement.Application.DTOs;
using Microsoft.Extensions.Logging;
using ApartmentManagement.Application.Interfaces;
using ApartmentManagement.Domain.Entities;

namespace ApartmentManagement.Application.Services
{
    public class ApartmentService
    {
        private readonly iApartmentRepository _apartmentRepository;
        private readonly ILogger<ApartmentService> _logger;

        public ApartmentService(iApartmentRepository apartmentRepository, ILogger<ApartmentService> logger)
        {
            _apartmentRepository = apartmentRepository
                ?? throw new ArgumentNullException(nameof(apartmentRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IReadOnlyList<ApartmentDto>> GetApartmentsAsync()
        {
            var apartments = await _apartmentRepository.GetAllAsync();

            var result = apartments.Select(a => new ApartmentDto
            {
                Id = a.Id,
                Name = a.Name,
                Address = a.Address,
                FlatCount = a.Flats.Count
            }).ToList();

            _logger.LogInformation("Retrieved {Count} apartments", result.Count);

            return result;
        }

        public async Task AddApartmentAsync(string name, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Apartment name is required.", nameof(name));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Apartment address is required.", nameof(address));

            if (name.Length > ApartmentManagement.Application.Validation.ValidationConstants.MaxNameLength)
                throw new ArgumentException($"Apartment name must be at most {ApartmentManagement.Application.Validation.ValidationConstants.MaxNameLength} characters.", nameof(name));

            var apartment = new Apartment(name, address);
            await _apartmentRepository.AddAsync(apartment);
            _logger.LogInformation("Added apartment {Name}", name);
        }

        public async Task UpdateApartmentAsync(int id, string name, string address)
        {
            if (id <= 0) throw new ArgumentException("Invalid id", nameof(id));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required", nameof(name));
            if (string.IsNullOrWhiteSpace(address)) throw new ArgumentException("Address is required", nameof(address));

            var apartment = await _apartmentRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Apartment not found.");

            apartment.UpdateDetails(name, address);
            await _apartmentRepository.UpdateAsync(apartment);
            _logger.LogInformation("Updated apartment {Id}", id);
        }

        public async Task DeleteApartmentAsync(int id)
        {
            if (id <= 0) throw new ArgumentException("Invalid id", nameof(id));

            var apartment = await _apartmentRepository.GetByIdAsync(id)
                ?? throw new InvalidOperationException("Apartment not found.");

            await _apartmentRepository.DeleteAsync(apartment);
            _logger.LogInformation("Deleted apartment {Id}", id);
        }
    }
}
