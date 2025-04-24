using Core.ViewModels;

namespace Core.Interfaces.Services;
public interface IAttendanceService
{
    public Task<IEnumerable<AttendanceViewModel>> GetAll();

    public Task<AttendanceViewModel> CreateAsync(int employeeId);

    public Task<AttendanceViewModel> UpdateAsync(int employeeId, int attendanceId);

    public Task<bool> DeleteOneAsync(int id);

}
