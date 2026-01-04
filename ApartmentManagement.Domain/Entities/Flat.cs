namespace ApartmentManagement.Domain.Entities
{
    public class Flat
    {
        public int Id { get; private set; }
        public string FlatNumber { get; private set; }
        public int Floor { get; private set; }

        public int ApartmentId { get; private set; }

        protected Flat() { }

        public Flat(string flatNumber, int floor, int apartmentId)
        {
            FlatNumber = flatNumber;
            Floor = floor;
            ApartmentId = apartmentId;
        }
    }
}
