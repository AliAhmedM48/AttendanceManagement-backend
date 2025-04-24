using Core.Enums;

namespace Core.ViewModels;
public class EmployeeViewModel
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string Governorate { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string? SignaturePath { get; set; }


    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; } = default(DateTime);

}

public class EmployeeCreateViewModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public string? SignaturePath { get; set; } = string.Empty;
}

public class EmployeeUpdateViewModel
{
    public string? FirstName { get; set; } = string.Empty;
    public string? LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; } = string.Empty;
    public string? Email { get; set; } = string.Empty;
    public string? Password { get; set; } = string.Empty;
    public string? NationalId { get; set; } = string.Empty;
    public string? SignaturePath { get; set; }
}

public class NationalIdInfo
{
    public DateTime BirthDate { get; set; }
    public string Governorate { get; set; } = string.Empty;
    public UserGender Gender { get; set; }
}
