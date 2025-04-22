using Core.ViewModels;

namespace Core.Interfaces.Services;
public interface IAttendanceService
{
    public Task<IEnumerable<AttendanceViewModel>> GetAll();

    public Task<AttendanceViewModel> CreateAsync(AttendanceCreateViewModel attendanceCreateViewModel);

    public Task<bool> UpdateAsync(int id);

    public Task<bool> DeleteOneAsync(int id);
}
