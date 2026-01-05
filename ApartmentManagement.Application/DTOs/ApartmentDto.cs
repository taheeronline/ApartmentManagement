namespace ApartmentManagement.Application.DTOs
{
    public class ApartmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int FlatCount { get; set; }
    }

}
