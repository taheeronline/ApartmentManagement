using ApartmentManagement.Application.Interfaces;
using ApartmentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            _residentRepository = residentRepository;
            _flatRepository = flatRepository;
        }

        public async Task AddResidentAsync(
            string fullName,
            string phoneNumber,
            string email,
            int flatId,
            ResidentType residentType)
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

        public async Task MoveOutResidentAsync(int residentId)
        {
            var resident = await _residentRepository.GetByIdAsync(residentId);
            if (resident == null) return;

            resident.MoveOut();
            await _residentRepository.UpdateAsync(resident);
        }

        public Task<IReadOnlyList<Resident>> GetResidentsByFlatAsync(int flatId)
        {
            return _residentRepository.GetByFlatAsync(flatId);
        }

        public Task DeleteResidentAsync(int residentId)
        {
            return _residentRepository.DeleteAsync(residentId);
        }
    }

}
