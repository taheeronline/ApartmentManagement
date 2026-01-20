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
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get apartments");
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
                _logger.LogInformation("Added apartment {Name}", name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add apartment {Name}", name);
                throw;
            }
        }
    }
}
