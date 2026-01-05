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
            _flatRepository = flatRepository;
        }

        public async Task<IReadOnlyList<FlatDto>> GetFlatsByApartmentAsync(int apartmentId)
        {
            var flats = await _flatRepository.GetByApartmentIdAsync(apartmentId);

            return flats.Select(f => new FlatDto
            {
                Id = f.Id,
                FlatNumber = f.FlatNumber,
                Floor = f.Floor
            }).ToList();
        }


        public async Task AddFlatAsync(string flatNumber, int floor, int apartmentId)
        {
            if (await _flatRepository.ExistsAsync(apartmentId, flatNumber))
            {
                throw new InvalidOperationException(
                    $"Flat {flatNumber} already exists in this apartment.");
            }

            var flat = new Flat(flatNumber, floor, apartmentId);
            await _flatRepository.AddAsync(flat);
        }

        public async Task DeleteFlatAsync(int id)
        {
            await _flatRepository.DeleteAsync(id);
        }

    }
}
