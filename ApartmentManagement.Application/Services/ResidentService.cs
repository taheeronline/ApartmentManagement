using System.Linq;
using Microsoft.Extensions.Logging;
using ApartmentManagement.Application.DTOs;
using ApartmentManagement.Application.Interfaces;
using ApartmentManagement.Domain.Entities;

namespace ApartmentManagement.Application.Services
{
    public class ResidentService
    {
        private readonly iResidentRepository _residentRepository;
        private readonly iFlatRepository _flatRepository;
        private readonly ILogger<ResidentService> _logger;

        private const int DefaultMaxOccupancy = 5;

        public ResidentService(
            iResidentRepository residentRepository,
            iFlatRepository flatRepository,
            ILogger<ResidentService> logger)
        {
            _residentRepository = residentRepository
                ?? throw new ArgumentNullException(nameof(residentRepository));
            _flatRepository = flatRepository
                ?? throw new ArgumentNullException(nameof(flatRepository));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task AddResidentAsync(
            string fullName,
            string phoneNumber,
            string email,
            int flatId,
            ResidentType residentType)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Resident full name is required.", nameof(fullName));

            // validate name length
            if (fullName.Length > ApartmentManagement.Application.Validation.ValidationConstants.MaxNameLength)
                throw new ArgumentException($"Resident full name must be at most {ApartmentManagement.Application.Validation.ValidationConstants.MaxNameLength} characters.", nameof(fullName));

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.", nameof(phoneNumber));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (flatId <= 0)
                throw new ArgumentException("Invalid flat id.", nameof(flatId));

            var flat = await _flatRepository.GetByIdAsync(flatId);
            if (flat == null)
                throw new InvalidOperationException("Flat does not exist.");

            var activeCount = await _residentRepository.GetActiveResidentCountByFlatAsync(flatId);

            if (activeCount >= DefaultMaxOccupancy)
                throw new InvalidOperationException("Flat occupancy limit reached.");

            var resident = new Resident(
                fullName,
                phoneNumber,
                email,
                flatId,
                residentType);

            await _residentRepository.AddAsync(resident);
            _logger.LogInformation("Added resident {FullName} to flat {FlatId}", fullName, flatId);
        }

        public async Task MoveOutResidentAsync(int residentId)
        {
            if (residentId <= 0)
                throw new ArgumentException("Invalid resident id.", nameof(residentId));

            var resident = await _residentRepository.GetByIdAsync(residentId);
            if (resident == null)
            {
                _logger.LogWarning("Attempt to move out non-existing resident {ResidentId}", residentId);
                throw new InvalidOperationException("Resident not found.");
            }

            if (!resident.IsActive)
                throw new InvalidOperationException("Resident is already moved out.");

            resident.MoveOut();
            await _residentRepository.UpdateAsync(resident);
        }

        public async Task<IReadOnlyList<Resident>> GetResidentsByFlatAsync(int flatId)
        {
            if (flatId <= 0)
                throw new ArgumentException("Invalid flat id.", nameof(flatId));

            return await _residentRepository.GetByFlatAsync(flatId);
        }

        public async Task<IReadOnlyList<ResidentDto>> GetAllAsync()
        {
            var residents = await _residentRepository.GetAllAsync();

            return residents.Select(r => new ResidentDto
            {
                Id = r.Id,
                FullName = r.FullName,
                PhoneNumber = r.PhoneNumber,
                Email = r.Email,
                FlatId = r.FlatId,
                FlatNumber = r.Flat.FlatNumber,
                Floor = r.Flat.Floor,
                ResidentType = r.ResidentType,
                MoveInDate = r.MoveInDate,
                MoveOutDate = r.MoveOutDate
            }).ToList();
        }

        public async Task DeleteResidentAsync(int residentId)
        {
            if (residentId <= 0)
                throw new ArgumentException("Invalid resident id.", nameof(residentId));

            await _residentRepository.DeleteAsync(residentId);
            _logger.LogInformation("Deleted resident {ResidentId}", residentId);
        }
    }
}
