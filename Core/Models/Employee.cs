namespace Core.Models;
public class Employee : BaseModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public int Age { get; set; }
    public string? SignaturePath { get; set; }

    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
