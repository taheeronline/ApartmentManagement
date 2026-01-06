using ApartmentManagement.Application.Interfaces;
using ApartmentManagement.Domain.Entities;

namespace ApartmentManagement.Application.Services
{
    public class ResidentService
    {
        private readonly iResidentRepository _residentRepository;
        private readonly iFlatRepository _flatRepository;

        private const int DefaultMaxOccupancy = 5;

        public ResidentService(
            iResidentRepository residentRepository,
            iFlatRepository flatRepository)
        {
            _residentRepository = residentRepository
                ?? throw new ArgumentNullException(nameof(residentRepository));

            _flatRepository = flatRepository
                ?? throw new ArgumentNullException(nameof(flatRepository));
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

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ArgumentException("Phone number is required.", nameof(phoneNumber));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required.", nameof(email));

            if (flatId <= 0)
                throw new ArgumentException("Invalid flat id.", nameof(flatId));

            try
            {
                var flat = await _flatRepository.GetByIdAsync(flatId);
                if (flat == null)
                    throw new InvalidOperationException("Flat does not exist.");

                var activeCount =
                    await _residentRepository.GetActiveResidentCountByFlatAsync(flatId);

                if (activeCount >= DefaultMaxOccupancy)
                    throw new InvalidOperationException("Flat occupancy limit reached.");

                var resident = new Resident(
                    fullName,
                    phoneNumber,
                    email,
                    flatId,
                    residentType);

                await _residentRepository.AddAsync(resident);
            }
            catch
            {
                throw;
            }
        }

        public async Task MoveOutResidentAsync(int residentId)
        {
            if (residentId <= 0)
                throw new ArgumentException("Invalid resident id.", nameof(residentId));

            try
            {
                var resident = await _residentRepository.GetByIdAsync(residentId);
                if (resident == null)
                    throw new InvalidOperationException("Resident not found.");

                if (!resident.IsActive)
                    throw new InvalidOperationException("Resident is already moved out.");

                resident.MoveOut();
                await _residentRepository.UpdateAsync(resident);
            }
            catch
            {
                throw;
            }
        }

        public async Task<IReadOnlyList<Resident>> GetResidentsByFlatAsync(int flatId)
        {
            if (flatId <= 0)
                throw new ArgumentException("Invalid flat id.", nameof(flatId));

            try
            {
                return await _residentRepository.GetByFlatAsync(flatId);
            }
            catch
            {
                throw;
            }
        }

        public async Task DeleteResidentAsync(int residentId)
        {
            if (residentId <= 0)
                throw new ArgumentException("Invalid resident id.", nameof(residentId));

            try
            {
                await _residentRepository.DeleteAsync(residentId);
            }
            catch
            {
                throw;
            }
        }
    }
}
