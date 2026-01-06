using ApartmentManagement.Application.DTOs;
using ApartmentManagement.Application.Interfaces;
using ApartmentManagement.Domain.Entities;

namespace ApartmentManagement.Application.Services
{
    public class FlatService
    {
        private readonly iFlatRepository _flatRepository;

        public FlatService(iFlatRepository flatRepository)
        {
            _flatRepository = flatRepository
                ?? throw new ArgumentNullException(nameof(flatRepository));
        }

        public async Task<IReadOnlyList<FlatDto>> GetFlatsByApartmentAsync(int apartmentId)
        {
            if (apartmentId <= 0)
                throw new ArgumentException("Invalid apartment id.", nameof(apartmentId));

            try
            {
                var flats = await _flatRepository.GetByApartmentIdAsync(apartmentId);

                return flats.Select(f => new FlatDto
                {
                    Id = f.Id,
                    FlatNumber = f.FlatNumber,
                    Floor = f.Floor
                }).ToList();
            }
            catch
            {
                // Preserve original stack trace
                throw;
            }
        }

        public async Task<IReadOnlyList<FlatDto>> GetAllAsync()
        {
            try
            {
                var flats = await _flatRepository.GetAllAsync();

                return flats.Select(f => new FlatDto
                {
                    Id = f.Id,
                    FlatNumber = f.FlatNumber,
                    Floor = f.Floor
                }).ToList();
            }
            catch
            {
                throw;
            }
        }

        public async Task AddFlatAsync(string flatNumber, int floor, int apartmentId)
        {
            if (string.IsNullOrWhiteSpace(flatNumber))
                throw new ArgumentException("Flat number is required.", nameof(flatNumber));

            if (floor < 0)
                throw new ArgumentException("Floor cannot be negative.", nameof(floor));

            if (apartmentId <= 0)
                throw new ArgumentException("Invalid apartment id.", nameof(apartmentId));

            try
            {
                var exists = await _flatRepository.ExistsAsync(apartmentId, flatNumber);
                if (exists)
                {
                    throw new InvalidOperationException(
                        $"Flat '{flatNumber}' already exists in this apartment.");
                }

                var flat = new Flat(flatNumber, floor, apartmentId);
                await _flatRepository.AddAsync(flat);
            }
            catch
            {
                throw;
            }
        }

        public async Task DeleteFlatAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid flat id.", nameof(id));

            try
            {
                await _flatRepository.DeleteAsync(id);
            }
            catch
            {
                throw;
            }
        }
    }
}
