using Core.Enums;

namespace Core.Models;
public class Employee : User
{

    public string PhoneNumber { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string? SignaturePath { get; set; }

    public string Governorate { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public UserGender Gender { get; set; }


    public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
}
