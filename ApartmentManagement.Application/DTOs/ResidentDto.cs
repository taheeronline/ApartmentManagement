using System;

namespace ApartmentManagement.Application.DTOs
{
    public class ResidentDto
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public int FlatId { get; set; }
        public string FlatNumber { get; set; } = string.Empty;
        public int Floor { get; set; }

        public ResidentType ResidentType { get; set; }

        public DateTime MoveInDate { get; set; }
        public DateTime? MoveOutDate { get; set; }

        public bool IsActive => MoveOutDate == null;
    }

}
