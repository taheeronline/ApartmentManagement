namespace ApartmentManagement.UI.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateOnly DateOfBirth{ get; set; }=DateOnly.FromDateTime(DateTime.Now);
    }
}
