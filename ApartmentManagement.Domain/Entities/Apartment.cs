namespace ApartmentManagement.Domain.Entities
{
    public class Apartment
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }

        private readonly List<Flat> _flats = new();
        public IReadOnlyCollection<Flat> Flats => _flats;

        protected Apartment() { }

        public Apartment(string name, string address)
        {
            Name = name;
            Address = address;
        }

        public void AddFlat(Flat flat)
        {
            _flats.Add(flat);
        }
    }
}
