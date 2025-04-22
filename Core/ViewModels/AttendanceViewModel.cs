namespace Core.ViewModels;
/*
 public class Attendance : BaseModel
{
    
}
 */
public class AttendanceViewModel
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }
    public DateTime CheckInTime { get; set; }
    public DateTime? CheckOutTime { get; set; } = null;
}

public class AttendanceCreateViewModel
{
    public int EmployeeId { get; set; }
}

