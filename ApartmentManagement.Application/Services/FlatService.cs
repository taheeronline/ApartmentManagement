using ApartmentManagement.Application.Interfaces;
using ApartmentManagement.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApartmentManagement.Application.Services
{
    public class FlatService
    {
        private readonly iFlatRepository _flatRepository;

        public FlatService(iFlatRepository flatRepository)
        {
            _flatRepository = flatRepository;
        }

        public Task<IReadOnlyList<Flat>> GetFlatsByApartmentAsync(int apartmentId)
        {
            return _flatRepository.GetByApartmentIdAsync(apartmentId);
        }

        public Task AddFlatAsync(string flatNumber, int floor, int apartmentId)
        {
            var flat = new Flat(flatNumber, floor, apartmentId);
            return _flatRepository.AddAsync(flat);
        }
    }
}
