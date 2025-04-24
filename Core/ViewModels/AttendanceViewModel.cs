namespace Core.ViewModels;

public class AttendanceViewModel
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; } = null;
    public double? TotalWorkedHours { get; set; } = null;
}

