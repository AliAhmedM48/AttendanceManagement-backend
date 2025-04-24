namespace Core.Models;

public class Attendance : BaseModel
{
    public int EmployeeId { get; set; }
    public Employee Employee { get; set; } = default!;
    public DateTime CheckInTime { get; set; } = DateTime.Now;
    public DateTime? CheckOutTime { get; set; }
    public double? TotalWorkedHours { get; set; }
}