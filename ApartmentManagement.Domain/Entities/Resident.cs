namespace ApartmentManagement.Domain.Entities
{
    public class Resident
    {
        public int Id { get; private set; }

        public string FullName { get; private set; } = null!;
        public string PhoneNumber { get; private set; } = null!;
        public string Email { get; private set; } = null!;

        public int FlatId { get; private set; }
        public Flat Flat { get; private set; } = null!;

        public ResidentType ResidentType { get; private set; }

        public DateTime MoveInDate { get; private set; }
        public DateTime? MoveOutDate { get; private set; }

        public bool IsActive => MoveOutDate == null;

        private Resident() { } // EF Core

        public Resident(
            string fullName,
            string phoneNumber,
            string email,
            int flatId,
            ResidentType residentType)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Email= email;
            FlatId = flatId;
            ResidentType = residentType;
            MoveInDate = DateTime.UtcNow;
        }

        public void MoveOut()
        {
            if (!IsActive) return;
            MoveOutDate = DateTime.UtcNow;
        }

        public void ChangeResidentType(ResidentType residentType)
        {
            ResidentType = residentType;
        }
    }


}
